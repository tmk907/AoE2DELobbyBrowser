using AoE2DELobbyNotifications.Api;
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

namespace AoE2DELobbyNotifications
{
    public class LobbySettings
    {
        public string Query { get; set; }
        public bool IsAutoRefreshEnabled { get; set; }
        public int Interval { get; set; }
        public string SelectedGameType { get; set; }
        public string SelectedGameSpeed { get; set; }
        public bool ShowNotifications { get; set; }
    }

    public class MainViewModel : ReactiveObject
    {
        private readonly Aoe2netApiClient _aoe2netApiClient;
        private readonly NotificationsService _notificationsService;
        private readonly SettingsService _settingsService;
        private readonly LobbySettings _lobbySettings;

        public MainViewModel()
        {
            _aoe2netApiClient = new Aoe2netApiClient();
            _notificationsService = new NotificationsService();
            _settingsService = new SettingsService();

            var defaultSettings = new LobbySettings
            {
                Interval = 30,
                IsAutoRefreshEnabled = true,
                Query = "",
                SelectedGameSpeed = GameSpeeds.First(),
                SelectedGameType = GameTypes.First(),
                ShowNotifications = false,
            };

            _lobbySettings = _settingsService.Get("lobby-settings", defaultSettings);

            SelectedGameType = _lobbySettings.SelectedGameType;
            SelectedGameSpeed = _lobbySettings.SelectedGameSpeed;
            Interval = _lobbySettings.Interval;
            ShowNotifications = _lobbySettings.ShowNotifications;
            Query = _lobbySettings.Query;
            IsAutoRefreshEnabled = _lobbySettings.IsAutoRefreshEnabled;

            this.WhenAnyPropertyChanged()
                .Do(_ => _settingsService.Save("lobby-settings", _lobbySettings))
                .Subscribe()
                .DisposeWith(Disposal);

            Func<Lobby, bool> queryFilter(string text) => lobby =>
            {
                return string.IsNullOrEmpty(text) || lobby.Name.ToLower().Contains(text.ToLower());
            };

            Func<Lobby, bool> gameTypeFilter(string gameType) => lobby =>
            {
                return gameType == GameType.All || lobby.GameType == gameType;
            };

            Func<Lobby, bool> gameSpeedFilter(string gameSpeed) => lobby =>
            {
                return gameSpeed == GameType.All || lobby.Speed == gameSpeed;
            };

            var filterQuery = this
                .WhenAnyValue(x => x.Query)
                .DistinctUntilChanged()
                .Select(queryFilter);

            var filterGameType = this
                .WhenAnyValue(x => x.SelectedGameType)
                .DistinctUntilChanged()
                .Select(gameTypeFilter);

            var filterGameSpeed = this
                .WhenAnyValue(x => x.SelectedGameSpeed)
                .DistinctUntilChanged()
                .Select(gameSpeedFilter);

            var all = _aoe2netApiClient
                .Connect()
                .Transform(dto => Lobby.Create(dto))
                .Filter(x => x.Name != "AUTOMATCH")
                .Do(x => Log.Information($"Before transform {DateTime.Now} Add: {x.Adds} Remove: {x.Removes} " +
                    $"Update:{x.Updates} Refresh: {x.Refreshes}"))
                .Publish().RefCount();

            var newLobbies = all
                .Skip(1)
                .OnItemAdded(x => x.IsNew = true)
                .WhereReasonsAre(ChangeReason.Add)
                .Filter(queryFilter(_query))
                .Filter(gameSpeedFilter(selectedGameSpeed))
                .Filter(gameTypeFilter(_selectedGameType))
                .Select(changeSet => changeSet.Select(x => x.Current).ToList())
                .Do(list => Log.Information($"Added {list.Count} new lobbies"))
                .Where(_ => ShowNotifications)
                .Do(x => _notificationsService.ShowNotifications(x))
                .Subscribe()
                .DisposeWith(Disposal);

            var filtered = all
                .Filter(filterQuery)
                .Filter(filterGameType)
                .Filter(filterGameSpeed)
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


        private string _selectedGameType;
        public string SelectedGameType
        {
            get => _selectedGameType;
            set => this.RaiseAndSetIfChanged(ref _selectedGameType, value);
        }

        private string selectedGameSpeed;
        public string SelectedGameSpeed
        {
            get => selectedGameSpeed;
            set => this.RaiseAndSetIfChanged(ref selectedGameSpeed, value);
        }

        private bool _showNotifications;
        public bool ShowNotifications
        {
            get => _showNotifications;
            set => this.RaiseAndSetIfChanged(ref _showNotifications, value);
        }
    }
}
