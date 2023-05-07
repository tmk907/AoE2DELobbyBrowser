using AoE2DELobbyBrowser.Api;
using AoE2DELobbyBrowser.Models;
using AoE2DELobbyBrowser.Services;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace AoE2DELobbyBrowser
{
    public class MainViewModel : ReactiveObject
    {
        private readonly IApiClient _apiClient;
        private readonly NotificationsService _notificationsService;
        private readonly AppSettingsService _settingsService;
        private readonly LobbySettings _lobbySettings;

        public MainViewModel()
        {
            _apiClient = App.ApiClient;
            _notificationsService = new NotificationsService();
            _settingsService = new AppSettingsService();

            var defaultSettings = new LobbySettings
            {
                Interval = 10,
                IsAutoRefreshEnabled = true,
                Query = "",
                Exclude = "",
                PlayerQuery = "",
                IsPlayerSearchEnabled = false,
                SelectedGameSpeed = GameSpeeds.First(),
                SelectedGameType = GameTypes.First(),
                SelectedMapType = MapTypes.First(),
                ShowNotifications = false,
            };

            _lobbySettings = _settingsService.Get("lobby-settings", defaultSettings);

            SelectedGameType = _lobbySettings.SelectedGameType;
            SelectedGameSpeed = _lobbySettings.SelectedGameSpeed;
            SelectedMapType = _lobbySettings.SelectedMapType;
            Interval = _lobbySettings.Interval;
            ShowNotifications = _lobbySettings.ShowNotifications;
            Query = _lobbySettings.Query;
            Excluded = _lobbySettings.Exclude;
            PlayerQuery = _lobbySettings.PlayerQuery;
            IsPlayerSearchEnabled =_lobbySettings.IsPlayerSearchEnabled;
            IsAutoRefreshEnabled = _lobbySettings.IsAutoRefreshEnabled;

            this.WhenAnyPropertyChanged()
                .Do(_ =>
                {
                    _lobbySettings.Query = Query;
                    _lobbySettings.Exclude = Excluded;
                    _lobbySettings.PlayerQuery = PlayerQuery;
                    _lobbySettings.IsPlayerSearchEnabled = IsPlayerSearchEnabled;
                    _lobbySettings.Interval = Interval;
                    _lobbySettings.IsAutoRefreshEnabled = IsAutoRefreshEnabled;
                    _lobbySettings.SelectedGameSpeed = SelectedGameSpeed;
                    _lobbySettings.SelectedGameType = SelectedGameType;
                    _lobbySettings.SelectedMapType = SelectedMapType;
                    _lobbySettings.ShowNotifications = ShowNotifications;
                    _settingsService.Save("lobby-settings", _lobbySettings);
                })
                .Subscribe()
                .DisposeWith(Disposal);

            Func<Lobby, bool> lobbyFilter = lobby =>
            {
                var isMatched = true;
                if (!string.IsNullOrEmpty(Query))
                {
                    var queries = Query.Split(AppSettings.Separator, StringSplitOptions.RemoveEmptyEntries);
                    isMatched = queries
                    .Any(x => lobby.Name.ToLowerInvariant().Contains(x.ToLowerInvariant()));
                }

                var isExcluded = false;
                if (!string.IsNullOrEmpty(Excluded))
                {
                    var exclusions = Excluded.Split(AppSettings.Separator, StringSplitOptions.RemoveEmptyEntries);
                    isExcluded = exclusions
                        .Any(x => lobby.Name.ToLowerInvariant().Contains(x.ToLowerInvariant()));
                }

                var hasPlayer = true;
                if (IsPlayerSearchEnabled && !string.IsNullOrEmpty(PlayerQuery))
                {
                    var players = PlayerQuery.Split(AppSettings.Separator, StringSplitOptions.RemoveEmptyEntries);
                    hasPlayer = players.Any(x => lobby.ContainsPlayer(x));
                }

                return isMatched && !isExcluded && hasPlayer;
            };

            Func<Lobby, bool> gameFilter = lobby =>
            {
                return (SelectedGameType == GameType.All || lobby.GameType == SelectedGameType)
                    && (SelectedGameSpeed == GameType.All || lobby.Speed == SelectedGameSpeed)
                    && (SelectedMapType == MapType.All || lobby.Map == SelectedMapType
                        || SelectedGameType == GameType.Scenario);
            };

            var lobbyObservableFilter = this
                .WhenAnyValue(x => x.Query, x => x.Excluded, x => x.PlayerQuery, x => x.IsPlayerSearchEnabled)
                .Do(x => Log.Debug($"Filter lobby {x}"))
                .DistinctUntilChanged()
                .Select(_ => lobbyFilter);

            var gameObservableFilter = this
                .WhenAnyValue(x => x.SelectedGameType, x => x.SelectedGameSpeed, x => x.SelectedMapType)
                .Do(x => Log.Debug($"Filter game {x}"))
                .DistinctUntilChanged()
                .Select(_ => gameFilter);

            var all = _apiClient
                .Connect()
                .Filter(x => x.Name != "AUTOMATCH")
                .Do(x => Log.Debug($"Before transform {DateTime.Now} Add: {x.Adds} Remove: {x.Removes} " +
                    $"Update:{x.Updates} Refresh: {x.Refreshes}"))
                .DisposeMany()
                .Publish().RefCount();

            var newLobbies = all
                .Skip(1)
                .OnItemAdded(x => x.IsNew = true)
                .WhereReasonsAre(ChangeReason.Add)
                .Filter(gameObservableFilter)
                .Filter(lobbyObservableFilter)
                .Select(changeSet => changeSet.Select(x => x.Current).ToList())
                .Do(list => Log.Debug($"Notification: {list.Count} new lobbies"))
                .Where(_ => ShowNotifications)
                .Do(x => _notificationsService.ShowNotifications(x))
                .Subscribe()
                .DisposeWith(Disposal);

            var myAdaptor = new MySortedObservableCollectionAdaptor();
            var filtered = all
                .Filter(gameObservableFilter)
                .Filter(lobbyObservableFilter)
                .Sort(SortExpressionComparer<Lobby>.Ascending(t => t.Name))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _lobbies, adaptor:myAdaptor)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(Disposal);

            this.RefreshCommand = ReactiveCommand.CreateFromTask(ct => _apiClient.Refresh(ct));

            this.WhenAnyObservable(x => x.RefreshCommand.IsExecuting)
                .StartWith(false)
                .DistinctUntilChanged()
                .ToProperty(this, x => x.Loading, out _loading)
                .DisposeWith(Disposal);

            this
                .WhenAnyValue(x => x.Interval, x => x.IsAutoRefreshEnabled)
                .Where(x => x.Item2)
                .Select(x => x.Item1)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Do(_ => Log.Debug("value changed"))
                .Select(interval =>
                    Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(interval), RxApp.TaskpoolScheduler)
                        .Do(x => Log.Debug($"Inner observable interval {x}"))
                        .Select(time => Unit.Default)
                    )
                .Switch()
                .Where(_ => _isAutoRefreshEnabled)
                .Do(_ => Log.Debug("Refresh enabled"))
                .InvokeCommand(this, x => x.RefreshCommand)
                .DisposeWith(Disposal);

            AddFriendCommand = ReactiveCommand.CreateFromTask<Player>(x => App.PlayersService.AddFriendAsync(x.SteamProfileId));
        }

        protected CompositeDisposable Disposal = new CompositeDisposable();
        public void Dispose()
        {
            Disposal.Dispose();
            _apiClient.Dispose();
        }

        public ReactiveCommand<Unit, Unit> RefreshCommand { get; }
        public ReactiveCommand<Player, Unit> AddFriendCommand { get; }


        private readonly ReadOnlyObservableCollection<Lobby> _lobbies;
        public ReadOnlyObservableCollection<Lobby> Lobbies => _lobbies;

        private string _query;
        public string Query
        {
            get => _query;
            set => this.RaiseAndSetIfChanged(ref _query, value);
        }

        private string _excluded;
        public string Excluded
        {
            get => _excluded;
            set => this.RaiseAndSetIfChanged(ref _excluded, value);
        }

        private string _playerQuery;
        public string PlayerQuery
        {
            get => _playerQuery;
            set => this.RaiseAndSetIfChanged(ref _playerQuery, value);
        }

        private bool _isPlayerSearchEnabled;
        public bool IsPlayerSearchEnabled
        {
            get => _isPlayerSearchEnabled;
            set => this.RaiseAndSetIfChanged(ref _isPlayerSearchEnabled, value);
        }

        private int _interval;
        public int Interval
        {
            get => _interval;
            set => this.RaiseAndSetIfChanged(ref _interval, value);
        }

        private bool _isAutoRefreshEnabled;
        public bool IsAutoRefreshEnabled
        {
            get => _isAutoRefreshEnabled;
            set => this.RaiseAndSetIfChanged(ref _isAutoRefreshEnabled, value);
        }

        private readonly ObservableAsPropertyHelper<bool> _loading;
        public bool Loading => _loading.Value;

        public List<string> GameTypes { get; } = new GameType().GetAll();
        public List<string> GameSpeeds { get; } = new GameSpeed().GetAll();
        public List<string> MapTypes { get; } = new MapType().GetAll();


        private string _selectedGameType;
        public string SelectedGameType
        {
            get => _selectedGameType;
            set => this.RaiseAndSetIfChanged(ref _selectedGameType, value);
        }

        private string _selectedGameSpeed;
        public string SelectedGameSpeed
        {
            get => _selectedGameSpeed;
            set => this.RaiseAndSetIfChanged(ref _selectedGameSpeed, value);
        }

        private string _selectedMapType;
        public string SelectedMapType
        {
            get => _selectedMapType;
            set => this.RaiseAndSetIfChanged(ref _selectedMapType, value);
        }

        private bool _showNotifications;
        public bool ShowNotifications
        {
            get => _showNotifications;
            set => this.RaiseAndSetIfChanged(ref _showNotifications, value);
        }
    }
}
