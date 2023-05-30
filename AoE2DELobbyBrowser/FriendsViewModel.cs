using AoE2DELobbyBrowser.Models;
using AoE2DELobbyBrowser.Services;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowser
{
    public class FriendsViewModel : ReactiveObject
    {
        private readonly IPlayersService _playersService;

        public FriendsViewModel() 
        {
            _playersService = App.PlayersService;

            var canExecuteAdd = this.WhenAnyValue(x => x.SteamId,
                (id) => _playersService.IsValidId(id));

            this.AddFriendFromIdCommand = ReactiveCommand.CreateFromTask(AddFriendAsync, canExecuteAdd);
            this.AddFriendCommand = ReactiveCommand.CreateFromTask<Player>(
                x => App.PlayersService.AddFriendAsync(x.SteamProfileId));
            this.RefreshCommand = ReactiveCommand.CreateFromTask(ct => RefreshAsync(ct));
            this.DeleteFriendCommand = new AsyncRelayCommand<Friend>(x=>DeleteAsync(x));

            var allFriends = App.LobbyService.FriendsChanges
                .Connect()
                .Sort(SortExpressionComparer<Friend>.Ascending(x => x.IsOnline ? 0 : 1).ThenByAscending(x => x.Player.Name))
                .TreatMovesAsRemoveAdd()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _friends)
                .Subscribe()
                .DisposeWith(Disposal);

            Observable
                .FromAsync(_playersService.UpdatePlayersFromSteamAsync)
                .Subscribe()
                .DisposeWith(Disposal);
        }

        protected CompositeDisposable Disposal = new CompositeDisposable();
        public void Dispose()
        {
            Disposal.Dispose();
        }

        public ReactiveCommand<Unit, Unit> AddFriendFromIdCommand { get; }
        public ReactiveCommand<Player, Unit> AddFriendCommand { get; }

        public ReactiveCommand<Unit, Unit> RefreshCommand { get; }
        public IAsyncRelayCommand<Friend>  DeleteFriendCommand { get; }

        private string _steamId;
        public string SteamId
        {
            get => _steamId;
            set => this.RaiseAndSetIfChanged(ref _steamId, value);
        }

        private readonly ReadOnlyObservableCollection<Friend> _friends;
        public ReadOnlyObservableCollection<Friend> Friends => _friends;

        private async Task AddFriendAsync()
        {
            await _playersService.AddFriendAsync(SteamId);
            SteamId = "";
        }

        private async Task DeleteAsync(Friend friend)
        {
            await _playersService.RemoveFriendAsync(friend.Player.SteamProfileId);
        }

        private async Task RefreshAsync(CancellationToken ct)
        {
            await _playersService.UpdatePlayersFromSteamAsync();
            await App.LobbyService.RefreshAsync(ct);
        }
    }
}
