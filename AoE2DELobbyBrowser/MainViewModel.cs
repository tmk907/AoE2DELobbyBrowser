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
        private readonly LobbyService _lobbyService;

        public MainViewModel()
        {
            _lobbyService = App.LobbyService;

            Settings = new AppSettingsService().GetLobbySettings();

            Settings.WhenAnyPropertyChanged()
                .Do(_ => Log.Debug($"{nameof(MainViewModel)} {nameof(Settings)} changed"))
                .Do(x => _lobbyService.UpdateSettings(x))
                .Subscribe()
                .DisposeWith(Disposal);

            var myAdaptor = new MySortedObservableCollectionAdaptor();
            var filtered = _lobbyService.FilteredLobbyChanges
                .Sort(SortExpressionComparer<Lobby>.Ascending(t => t.Name))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _lobbies, adaptor:myAdaptor)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(Disposal);

            this.RefreshCommand = ReactiveCommand.CreateFromTask(
                ct => _lobbyService.RefreshAsync(ct),
                _lobbyService.IsLoading.Select(x => !x));
            this.AddFriendCommand = ReactiveCommand.CreateFromTask<Player>(
                x => App.PlayersService.AddFriendAsync(x.SteamProfileId));

            _lobbyService.IsLoading
                .DistinctUntilChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.Loading, out _loading)
                .DisposeWith(Disposal);

            Observable
                .FromAsync(ct => _lobbyService.RefreshAsync(ct))
                .Subscribe()
                .DisposeWith(Disposal);
        }

        protected CompositeDisposable Disposal = new CompositeDisposable();
        public void Dispose()
        {
            Disposal.Dispose();
        }

        public ReactiveCommand<Unit, Unit> RefreshCommand { get; }
        public ReactiveCommand<Player, Unit> AddFriendCommand { get; }

        public LobbySettings Settings { get; private set; }


        private readonly ReadOnlyObservableCollection<Lobby> _lobbies;
        public ReadOnlyObservableCollection<Lobby> Lobbies => _lobbies;


        private readonly ObservableAsPropertyHelper<bool> _loading;
        public bool Loading => _loading.Value;

        public List<string> GameTypes { get; } = new GameType().GetAll();
        public List<string> GameSpeeds { get; } = new GameSpeed().GetAll();
        public List<string> MapTypes { get; } = new MapType().GetAll();
    }
}
