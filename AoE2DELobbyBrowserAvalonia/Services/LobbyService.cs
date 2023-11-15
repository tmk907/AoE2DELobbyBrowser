using AoE2DELobbyBrowserAvalonia.Api;
using AoE2DELobbyBrowserAvalonia.Models;
using Serilog;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Reactive.Concurrency;
using DynamicData.Binding;
using System.Threading;
using DynamicData;
using Avalonia.ReactiveUI;

namespace AoE2DELobbyBrowserAvalonia.Services
{
    public class LobbyService
    {
        private readonly IApiClient _apiClient;
        private readonly AppSettingsService _appSettingsService;
        private readonly INotificationsService _notificationsService;
        private LobbySettings _settings;

        private ISubject<bool> _isLoadingSubject;

        protected CompositeDisposable Disposal = new CompositeDisposable();
        private readonly SourceCache<LobbyVM, string> _itemsCache;

        public LobbyService(IApiClient apiClient, AppSettingsService appSettingsService, INotificationsService notificationsService)
        {
            _apiClient = apiClient;
            _appSettingsService = appSettingsService;
            _notificationsService = notificationsService;
            _settings = appSettingsService.GetLobbySettings();
            _itemsCache = new SourceCache<LobbyVM, string>(x => x.MatchId);
            _isLoadingSubject = new Subject<bool>();
            
            IsLoading = _isLoadingSubject.StartWith(false).Replay(1).AutoConnect();

            _settings.WhenAnyPropertyChanged()
                .Do(_ => Log.Debug($"Settings changed"))
                .Do(x => _appSettingsService.SaveLobbySettings(x))
                .Subscribe()
                .DisposeWith(Disposal);

            _settings
                .WhenAnyPropertyChanged(new[] { nameof(_settings.Interval), nameof(_settings.IsAutoRefreshEnabled) })
                .Select(s => (s.Interval, s.IsAutoRefreshEnabled))
                .StartWith((_settings.Interval, _settings.IsAutoRefreshEnabled))
                .Select(x => x.Interval)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Select(interval =>
                    Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(interval), ThreadPoolScheduler.Instance)
                        .Do(x => Log.Debug($"Inner observable interval {x}"))
                        .Select(time => Unit.Default)
                    )
                .Switch()
                .Where(_ => _settings.IsAutoRefreshEnabled)
                .Do(_ => Log.Debug("Refresh enabled"))
                .Select(_ => Observable.FromAsync(ct => RefreshAsync(ct)))
                .Merge()
                .Subscribe()
                .DisposeWith(Disposal);

            AllLobbyChanges = _itemsCache
                .Connect()
                .ObserveOn(ThreadPoolScheduler.Instance)
                .Do(_ => Log.Debug($"onNext {nameof(AllLobbyChanges)} "))
                .Filter(x => x.Name != "AUTOMATCH")
                .Do(x => Log.Debug($"Before transform {DateTime.Now} Add: {x.Adds} Remove: {x.Removes} " +
                    $"Update: {x.Updates} Refresh: {x.Refreshes}"))
                .DisposeMany()
                .Publish().RefCount();

            AllLobbyChanges
                .Skip(1)
                .ObserveOn(AvaloniaScheduler.Instance)
                .OnItemAdded(x => x.IsNew = true)
                .Subscribe()
                .DisposeWith(Disposal);

            Func<LobbyVM, bool> lobbyFilter = lobby =>
            {
                var isMatched = true;
                if (!string.IsNullOrEmpty(_settings.Query))
                {
                    var queries = _settings.Query.Split(AppSettings.Separator, StringSplitOptions.RemoveEmptyEntries);
                    isMatched = queries
                        .Any(x => lobby.Name.ToLowerInvariant().Contains(x.ToLowerInvariant()));
                }

                var isExcluded = false;
                if (!string.IsNullOrEmpty(_settings.Exclude))
                {
                    var exclusions = _settings.Exclude.Split(AppSettings.Separator, StringSplitOptions.RemoveEmptyEntries);
                    isExcluded = exclusions
                        .Any(x => lobby.Name.ToLowerInvariant().Contains(x.ToLowerInvariant()));
                }

                return isMatched && !isExcluded;
            };

            Func<LobbyVM, bool> gameFilter = lobby =>
            {
                return (_settings.SelectedGameType == GameType.All || lobby.GameType == _settings.SelectedGameType)
                    && (_settings.SelectedGameSpeed == GameType.All || lobby.Speed == _settings.SelectedGameSpeed)
                    && (_settings.SelectedMapType == MapType.All || lobby.Map == _settings.SelectedMapType
                        || _settings.SelectedGameType == GameType.Scenario);
            };

            var lobbyObservableFilter = _settings
                .WhenAnyPropertyChanged(new[] { nameof(_settings.Query), nameof(_settings.Exclude) })
                .Select(s => (s.Query, s.Exclude))
                .StartWith((_settings.Query, _settings.Exclude))
                .Do(x => Log.Debug($"Filter lobby {x.Query} {x.Exclude}"))
                .DistinctUntilChanged()
                .Select(_ => lobbyFilter);

            var gameObservableFilter = _settings
                .WhenAnyPropertyChanged(new[] { nameof(_settings.SelectedGameType), nameof(_settings.SelectedGameSpeed), nameof(_settings.SelectedMapType) })
                .Do(x => Log.Debug($"Filter game {x.SelectedGameType} {x.SelectedGameSpeed} {x.SelectedMapType}"))
                .Select(s => (s.SelectedGameType, s.SelectedGameSpeed, s.SelectedMapType))
                .StartWith((_settings.SelectedGameType, _settings.SelectedGameSpeed, _settings.SelectedMapType))
                .DistinctUntilChanged()
                .Select(_ => gameFilter);

            FilteredLobbyChanges = AllLobbyChanges
                .Filter(gameObservableFilter)
                .Filter(lobbyObservableFilter)
                .Do(_ => Log.Debug($"onNext {nameof(FilteredLobbyChanges)}"))
                .Publish().RefCount();

            AllLobbyChanges
                .Skip(1)
                .Where(_ => _settings.ShowNotifications)
                .WhereReasonsAre(ChangeReason.Add)
                .Select(changeSet => changeSet.Select(x => x.Current).ToList())
                .Do(list => Log.Debug($"Notification: {list.Count} new lobbies all"))
                .Select(x => x.Where(x => lobbyFilter(x)).ToList())
                .Do(list => Log.Debug($"Notification: {list.Count} new lobbies lobbyFilter"))
                .Select(x => x.Where(x => gameFilter(x)).ToList())
                .Do(list => Log.Debug($"Notification: {list.Count} new lobbies gameFilter"))
                .Select(lobby => Observable.FromAsync(_ => _notificationsService.ShowNotifications(lobby))
                    .Catch((Exception ex) =>
                    {
                        Log.Error(ex.ToString());
                        return Observable.Empty<Unit>();
                    }))
                .Concat()
                .Subscribe()
                .DisposeWith(Disposal);
        }

        public IObservable<bool> IsLoading { get; private set; }
        public IObservable<IChangeSet<LobbyVM, string>> AllLobbyChanges { get; private set; }
        public IObservable<IChangeSet<LobbyVM, string>> FilteredLobbyChanges { get; private set; }

        public void Dispose()
        {
            Disposal.Dispose();
        }

        public void UpdateSettings(LobbySettings settings)
        {
            _settings.Update(settings);
        }

        public async Task RefreshAsync(CancellationToken token)
        {
            _isLoadingSubject.OnNext(true);

            var results = await _apiClient.GetAllLobbiesAsync(token);
            var lobbies = results.Select(dto => LobbyVM.Create(dto));
            var keysToDelete = _itemsCache.Keys.ToHashSet();
            keysToDelete.ExceptWith(lobbies.Select(x => x.MatchId).ToList());
            _itemsCache.Edit(updater =>
            {
                updater.RemoveKeys(keysToDelete);
                updater.AddOrUpdate(lobbies);
            });

            _isLoadingSubject?.OnNext(false);
        }
    }
}