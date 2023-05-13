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
using System.Threading;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowser
{
    public class FriendsViewModel : ReactiveObject
    {
        private readonly IPlayersService _playersService;
        private readonly IApiClient _apiClient;

        public FriendsViewModel() 
        {
            _playersService = App.PlayersService;
            _apiClient = App.ApiClient;

            var canExecuteAdd = this.WhenAnyValue(x => x.SteamId,
                (id) => _playersService.IsValidId(id));
            this.AddFriendCommand = ReactiveCommand.CreateFromTask(AddFriendAsync, canExecuteAdd);
            this.RefreshCommand = ReactiveCommand.CreateFromTask(ct => RefreshAsync(ct));
            this.DeleteFriendCommand = ReactiveCommand.CreateFromTask<Friend>(x => DeleteAsync(x));

            var allFriends = _playersService.Items.Connect()
                .Transform(x => Friend.Create(x))
                .Do(x => Log.Debug("allfriends"))
                .Sort(SortExpressionComparer<Friend>.Ascending(x => x.Name))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _friends)
                .Subscribe()
                .DisposeWith(Disposal);

            Observable
                .FromAsync(_playersService.ReloadAsync)
                .Do(x=>Log.Debug("test"))
                .Subscribe()
                .DisposeWith(Disposal);
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
        }

        private async Task DeleteAsync(Friend friend)
        {
            await _playersService.RemoveFriendAsync(friend.SteamId);
        }

        private async Task RefreshAsync(CancellationToken ct)
        {
            await _playersService.ReloadAsync();
            await _apiClient.Refresh(ct);

            foreach (var friend in Friends)
            {
                friend.Lobby = _apiClient.Items.Items.FirstOrDefault(l => l.ContainsPlayer(friend.SteamId));
            }
        }
    }
}
