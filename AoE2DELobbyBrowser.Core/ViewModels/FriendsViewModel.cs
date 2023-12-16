using AoE2DELobbyBrowser.Core.Models;
using AoE2DELobbyBrowser.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowser.Core.ViewModels
{
    public partial class FriendsViewModel : ObservableObject
    {
        private readonly IPlayersService _playersService;
        private readonly LobbyService _lobbyService;
        private readonly IScheduler _uiScheduler;

        public FriendsViewModel() 
        {
            _playersService = Ioc.Default.GetRequiredService<IPlayersService>();
            _lobbyService = Ioc.Default.GetRequiredService<LobbyService>();
            _uiScheduler = Ioc.Default.GetRequiredService<IScheduler>();

            var isOnlineChanges = _lobbyService.FriendsChanges.Connect()
                .WhenPropertyChanged(x => x.IsOnline, false)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Select(_ => Unit.Default);
            var comparer = SortExpressionComparer<FriendVM>.Ascending(x => x.IsOnline ? 0 : 1).ThenByAscending(x => x.Player.Name);

            var allFriends = _lobbyService.FriendsChanges
                .Connect()
                .Sort(comparer, isOnlineChanges)
                .TreatMovesAsRemoveAdd()
                .ObserveOn(_uiScheduler)
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

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddFriendCommand))]
        private string _steamId;

        private bool CanAddFriend()
        {
            return _playersService.IsValidId(SteamId);
        }


        private readonly ReadOnlyObservableCollection<FriendVM> _friends;
        public ReadOnlyObservableCollection<FriendVM> Friends => _friends;


        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task AddFriendAsync(PlayerVM player)
        {
            await _playersService.AddFriendAsync(player.SteamProfileId);
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanAddFriend))]
        private async Task AddFriendFromIdAsync()
        {
            await _playersService.AddFriendAsync(SteamId);
            SteamId = "";
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task DeleteFriendAsync(FriendVM friend)
        {
            await _playersService.RemoveFriendAsync(friend.Player.SteamProfileId);
        }

        [RelayCommand(AllowConcurrentExecutions =false)]
        private async Task RefreshAsync(CancellationToken ct)
        {
            await _playersService.UpdatePlayersFromSteamAsync();
            await _lobbyService.RefreshAsync(ct);
        }
    }
}
