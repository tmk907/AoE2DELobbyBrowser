using AoE2DELobbyBrowser.Api;
using AoE2DELobbyBrowser.Models;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowser.Services
{
    public class LobbyService : ReactiveObject
    {
        private readonly IApiClient _apiClient;
        private readonly NotificationsService _notificationsService;
        private readonly IPlayersService _playersService;
        private readonly AppSettingsService _settingsService;

        private ISubject<bool> _isLoadingSubject;

        public LobbyService() 
        {
            _apiClient = App.ApiClient;
            _notificationsService = new NotificationsService();
            _playersService = App.PlayersService;
            _settingsService = new AppSettingsService();

            _settings = _settingsService.GetLobbySettings();

            _settings.WhenAnyPropertyChanged()
                .Do(_ => Log.Debug("Settings changed"))
                .Do(_ =>
                {
                    _settingsService.SaveLobbySettings(_settings);
                })
                .Subscribe()
                .DisposeWith(Disposal);

            _isLoadingSubject = new Subject<bool>();
            IsLoading = _isLoadingSubject.StartWith(false).Replay(1).AutoConnect();

            Func<Lobby, bool> lobbyFilter = lobby =>
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

            Func<Lobby, bool> gameFilter = lobby =>
            {
                return (_settings.SelectedGameType == GameType.All || lobby.GameType == _settings.SelectedGameType)
                    && (_settings.SelectedGameSpeed == GameType.All || lobby.Speed == _settings.SelectedGameSpeed)
                    && (_settings.SelectedMapType == MapType.All || lobby.Map == _settings.SelectedMapType
                        || _settings.SelectedGameType == GameType.Scenario);
            };

            var lobbyObservableFilter = this
                .WhenAnyValue(x => x._settings.Query, x => x._settings.Exclude)
                .Do(x => Log.Debug($"Filter lobby {x}"))
                .DistinctUntilChanged()
                .Select(_ => lobbyFilter);

            var gameObservableFilter = this
                .WhenAnyValue(x => x._settings.SelectedGameType, x => x._settings.SelectedGameSpeed, x => x._settings.SelectedMapType)
                .Do(x => Log.Debug($"Filter game {x}"))
                .DistinctUntilChanged()
                .Select(_ => gameFilter);

            AllLobbyChanges = _apiClient
                .Connect()
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Do(_ => Log.Debug($"onNext {nameof(AllLobbyChanges)} "))
                .Filter(x => x.Name != "AUTOMATCH")
                .Do(x => Log.Debug($"Before transform {DateTime.Now} Add: {x.Adds} Remove: {x.Removes} " +
                    $"Update: {x.Updates} Refresh: {x.Refreshes}"))
                .DisposeMany()
                .Publish().RefCount();

            AllLobbyChanges
                .Skip(1)
                .ObserveOn(RxApp.MainThreadScheduler)
                .OnItemAdded(x => x.IsNew = true)
                .Subscribe()
                .DisposeWith(Disposal);

            FilteredLobbyChanges = AllLobbyChanges
                .Filter(gameObservableFilter)
                .Filter(lobbyObservableFilter)
                .Do(_ => Log.Debug($"onNext {nameof(FilteredLobbyChanges)}"))
                .Publish().RefCount();

            var newLobbies = AllLobbyChanges
                .Skip(1)
                .Where(_ => _settings.ShowNotifications)
                .WhereReasonsAre(ChangeReason.Add)
                .Select(changeSet => changeSet.Select(x => x.Current).ToList())
                .Do(list => Log.Debug($"Notification: {list.Count} new lobbies all"))
                .Select(x => x.Where(x => lobbyFilter(x)).ToList())
                .Do(list => Log.Debug($"Notification: {list.Count} new lobbies lobbyFilter"))
                .Select(x => x.Where(x => gameFilter(x)).ToList())
                .Do(list => Log.Debug($"Notification: {list.Count} new lobbies gameFilter"))
                .Do(x => _notificationsService.ShowNotifications(x))
                .Subscribe()
                .DisposeWith(Disposal);

            _settings
                .WhenAnyValue(x => x.Interval, x => x.IsAutoRefreshEnabled)
                .Where(x => x.Item2)
                .Select(x => x.Item1)
                .Throttle(TimeSpan.FromMilliseconds(500))
                //.Do(_ => Log.Debug("value changed"))
                .Select(interval =>
                    Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(interval), RxApp.TaskpoolScheduler)
                        //.Do(x => Log.Debug($"Inner observable interval {x}"))
                        .Select(time => Unit.Default)
                    )
                .Switch()
                .Where(_ => _settings.IsAutoRefreshEnabled)
                .Do(_ => Log.Debug("Refresh enabled"))
                .Select(_ => Observable.FromAsync(ct => RefreshAsync(ct)))
                .Merge()
                .Subscribe()
                .DisposeWith(Disposal);

            AllLobbyChanges
                .Bind(out _allLobbies)
                .Subscribe() 
                .DisposeWith(Disposal);

            _friendSource = new SourceCache<Friend, string>(x => x.Player.SteamProfileId);
            _playersService.AllPlayers.Connect()
                .Do(x => Log.Debug("LobbyService AllPlayerChanges"))
                .ObserveOn(RxApp.MainThreadScheduler)
                .OnItemAdded(x =>
                {
                    var friend = Friend.Create(x);
                    friend.UpdateStatus(x.Status);
                    friend.UpdateLobby(_allLobbies.FirstOrDefault(x => x.ContainsPlayer(friend.Player.SteamProfileId)));
                    _friendSource.AddOrUpdate(friend);
                })
                .OnItemRemoved(x=>
                {
                    _friendSource.RemoveKey(x.SteamId);
                })
                .OnItemUpdated((current, prev) =>
                {
                    var friend = _friendSource.Lookup(current.SteamId);
                    if (friend.HasValue)
                    {
                        friend.Value.Player.UpdateCountry(current.LocCountryCode);
                        friend.Value.UpdateStatus(current.Status);
                    }
                })
                .Subscribe()
                .DisposeWith(Disposal);

            AllLobbyChanges
                .Do(_ => Log.Debug("LobbyService AllLobbyChanges ToCollection"))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(_ =>
                {
                    foreach (var friend in _friendSource.Items)
                    {
                        friend.UpdateLobby(_allLobbies.FirstOrDefault(x => x.ContainsPlayer(friend.Player.SteamProfileId)));
                        friend.Player.UpdateCountry(friend?.Lobby?.Players?.FirstOrDefault()?.Country);
                    }
                })
                .Subscribe()
                .DisposeWith(Disposal);

            FriendsChanges = _friendSource.AsObservableCache();
        }

        protected CompositeDisposable Disposal = new CompositeDisposable();
        public void Dispose()
        {
            Disposal.Dispose();
            _apiClient.Dispose();
        }

        private LobbySettings _settings;

        private readonly SourceCache<Friend, string> _friendSource;
        public IObservableCache<Friend, string> FriendsChanges { get; private set; }

        private readonly ReadOnlyObservableCollection<Lobby> _allLobbies;

        public IObservable<IChangeSet<Lobby,string>> AllLobbyChanges { get; private set; }
        public IObservable<IChangeSet<Lobby, string>> FilteredLobbyChanges { get; private set; }

        public IObservable<bool> IsLoading { get; private set; }

        public void UpdateSettings(LobbySettings settings)
        {
            _settings.Update(settings);
        }

        public async Task RefreshAsync(CancellationToken token)
        {
            _isLoadingSubject.OnNext(true);
            await _apiClient.Refresh(token);
            _isLoadingSubject?.OnNext(false);
        }
    }
}
