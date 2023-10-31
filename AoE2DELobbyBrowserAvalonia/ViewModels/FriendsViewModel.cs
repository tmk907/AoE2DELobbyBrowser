using AoE2DELobbyBrowser.Models;
using AoE2DELobbyBrowser.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AoE2DELobbyBrowser
{
    public partial class FriendsViewModel : ObservableObject
    {
        private readonly IPlayersService _playersService;

        public ICommand AddFriendFromIdCommand { get; }
        public AsyncRelayCommand<Player> AddFriendCommand { get; }

        public ICommand RefreshCommand { get; }
        public IAsyncRelayCommand<Friend>  DeleteFriendCommand { get; }

        [ObservableProperty]
        private string _steamId;

        public ReadOnlyObservableCollection<Friend> Friends { get; }

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
            //await App.LobbyService.RefreshAsync(ct);
        }
    }
}
