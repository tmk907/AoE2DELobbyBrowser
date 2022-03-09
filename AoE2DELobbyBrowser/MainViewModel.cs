using AoE2DELobbyBrowser.Api;
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
        private readonly Aoe2netApiClient _aoe2netApiClient;
        private readonly NotificationsService _notificationsService;
        private readonly AppSettingsService _settingsService;
        private readonly LobbySettings _lobbySettings;

        public MainViewModel()
        {
            _aoe2netApiClient = new Aoe2netApiClient();
            _notificationsService = new NotificationsService();
            _settingsService = new AppSettingsService();

            var defaultSettings = new LobbySettings
            {
                Interval = 30,
                IsAutoRefreshEnabled = true,
                Query = "",
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
            IsAutoRefreshEnabled = _lobbySettings.IsAutoRefreshEnabled;

            this.WhenAnyPropertyChanged()
                .Do(_ =>
                {
                    _lobbySettings.Query = Query;
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

            Func<Lobby, bool> queryFilter = lobby =>
            {
                return string.IsNullOrEmpty(Query) || lobby.Name.ToLower().Contains(Query.ToLower());
            };

            Func<Lobby, bool> gameTypeFilter = lobby =>
            {
                return SelectedGameType == GameType.All || lobby.GameType == SelectedGameType;
            };

            Func<Lobby, bool> gameSpeedFilter = lobby =>
            {
                return SelectedGameSpeed == GameType.All || lobby.Speed == SelectedGameSpeed;
            };

            Func<Lobby, bool> mapTypeFilter = lobby =>
            {
                return SelectedGameType == GameType.Scenario 
                || SelectedMapType == MapType.All || lobby.Map == SelectedMapType;
            };

            var filterQuery = this
                .WhenAnyValue(x => x.Query)
                .Do(x => Log.Information($"Filter Query  {x}"))
                .DistinctUntilChanged()
                .Select(_ => queryFilter);

            var filterGameType = this
                .WhenAnyValue(x => x.SelectedGameType)
                .Do(x => Log.Information($"Filter SelectedGameType  {x}"))
                .DistinctUntilChanged()
                .Select(_ => gameTypeFilter);

            var filterGameSpeed = this
                .WhenAnyValue(x => x.SelectedGameSpeed)
                .Do(x => Log.Information($"Filter SelectedGameSpeed  {x}"))
                .DistinctUntilChanged()
                .Select(_ => gameSpeedFilter);

            var filterMapType = this
                .WhenAnyValue(x => x.SelectedMapType)
                .Do(x => Log.Information($"Filter SelectedMapType  {x}"))
                .DistinctUntilChanged()
                .Select(_ => mapTypeFilter);

            var all = _aoe2netApiClient
                .Connect()
                .Transform(dto => Lobby.Create(dto))
                .Filter(x => x.Name != "AUTOMATCH")
                .Filter(x => x.IsUnknownOpenedAt || x.OpenedAt > DateTime.Now.AddHours(-12))
                .Do(x => Log.Information($"Before transform {DateTime.Now} Add: {x.Adds} Remove: {x.Removes} " +
                    $"Update:{x.Updates} Refresh: {x.Refreshes}"))
                .Publish().RefCount();

            var newLobbies = all
                .Skip(1)
                .OnItemAdded(x => x.IsNew = true)
                .WhereReasonsAre(ChangeReason.Add)
                .Filter(gameSpeedFilter)
                .Filter(gameTypeFilter)
                .Filter(mapTypeFilter)
                .Filter(queryFilter)
                .Select(changeSet => changeSet.Select(x => x.Current).ToList())
                .Do(list => Log.Information($"Notification: {list.Count} new lobbies"))
                .Where(_ => ShowNotifications)
                .Do(x => _notificationsService.ShowNotifications(x))
                .Subscribe()
                .DisposeWith(Disposal);

            var filtered = all
                .Filter(filterGameSpeed)
                .Filter(filterGameType)
                .Filter(filterMapType)
                .Filter(filterQuery)
                .Sort(SortExpressionComparer<Lobby>.Ascending(t => t.Name))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _lobbies)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(Disposal);

            this.RefreshCommand = ReactiveCommand.CreateFromTask(ct => _aoe2netApiClient.Refresh(ct));

            this.WhenAnyObservable(x => x.RefreshCommand.IsExecuting)
                .StartWith(false)
                .DistinctUntilChanged()
                .ToProperty(this, x => x.Loading, out _loading  )
                .DisposeWith(Disposal);

            this
                .WhenAnyValue(x => x.Interval, x => x.IsAutoRefreshEnabled)
                .Where(x => x.Item2)
                .Select(x => x.Item1)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Do(_ => Log.Information("value changed"))
                .Select(interval =>
                    Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(interval), RxApp.TaskpoolScheduler)
                        .Do(x => Log.Information($"Inner observable interval {x}"))
                        .Select(time => Unit.Default)
                    )
                .Switch()
                .Where(_ => _isAutoRefreshEnabled)
                .Do(_ => Log.Information("Refresh enabled"))
                .InvokeCommand(this, x => x.RefreshCommand)
                .DisposeWith(Disposal);
        }

        protected CompositeDisposable Disposal = new CompositeDisposable();
        public void Dispose()
        {
            Disposal.Dispose();
        }

        public ReactiveCommand<Unit,Unit> RefreshCommand { get; }

        private readonly ReadOnlyObservableCollection<Lobby> _lobbies;
        public ReadOnlyObservableCollection<Lobby> Lobbies => _lobbies;

        private string _query;
        public string Query
        {
            get => _query;
            set => this.RaiseAndSetIfChanged(ref _query, value);
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
