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
using System.Threading;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowser
{
    public class FriendsViewModel : ReactiveObject
    {
        private readonly IPlayersService _playersService;

        //private readonly SourceCache<Friend, string> _friendSource;

        public FriendsViewModel() 
        {
            _playersService = App.PlayersService;
            //_friendSource = new SourceCache<Friend, string>(x => x.SteamId);

            var canExecuteAdd = this.WhenAnyValue(x => x.SteamId,
                (id) => _playersService.IsValidId(id));

            this.AddFriendCommand = ReactiveCommand.CreateFromTask(AddFriendAsync, canExecuteAdd);
            this.RefreshCommand = ReactiveCommand.CreateFromTask(ct => RefreshAsync(ct));
            this.DeleteFriendCommand = ReactiveCommand.CreateFromTask<Friend>(x => DeleteAsync(x));

            var allFriends = App.LobbyService.FriendsChanges
                .Connect()
                .Sort(SortExpressionComparer<Friend>.Ascending(x => x.Name))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _friends)
                .Subscribe()
                .DisposeWith(Disposal);

            //App.LobbyService.AllLobbyChanges
            //    .ToCollection()
            //    .ObserveOn(RxApp.MainThreadScheduler)
            //    .Do(lobbies =>
            //    {
            //        foreach (var friend in _friendSource.Items)
            //        {
            //            friend.Lobby = lobbies.FirstOrDefault(x => x.ContainsPlayer(friend.SteamId));
            //        }
            //    })
            //    .Subscribe()
            //    .DisposeWith(Disposal);

            //Observable
            //    .FromAsync(ct => RefreshAsync(ct))
            //    .Do(x => Log.Debug("test"))
            //    .Subscribe()
            //    .DisposeWith(Disposal);
        }

        protected CompositeDisposable Disposal = new CompositeDisposable();
        public void Dispose()
        {
            Disposal.Dispose();
        }

        public ReactiveCommand<Unit, Unit> AddFriendCommand { get; }
        public ReactiveCommand<Unit, Unit> RefreshCommand { get; }
        public ReactiveCommand<Friend, Unit> DeleteFriendCommand { get; }

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
            //var list = await _playersService.GetFriendsListAsync();
            //_friendSource.AddOrUpdate(list.Select(x => Friend.Create(x)));
        }

        private async Task DeleteAsync(Friend friend)
        {
            //_friendSource.Remove(friend);
            await _playersService.RemoveFriendAsync(friend.SteamId);
        }

        private async Task RefreshAsync(CancellationToken ct)
        {
            var list = await _playersService.GetFriendsListAsync();
            //_friendSource.AddOrUpdate(list.Select(x => Friend.Create(x)));
        }
    }
}
