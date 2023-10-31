using AoE2DELobbyBrowser.Api;
using AoE2DELobbyBrowser.Services;
using AoE2DELobbyBrowserAvalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AoE2DELobbyBrowser.Models
{
    public partial class Lobby : ObservableObject
    {
        public Lobby()
        {
            JoinGameCommand = new AsyncRelayCommand(JoinGame);
            CopyLobbyLinkCommand = new AsyncRelayCommand(CopyJoinLinkToClipboard);
            AddedAt = DateTime.Now;
        }

        public string Name { get; private set; }

        [ObservableProperty]
        private int _numPlayers;

        [ObservableProperty]
        private int _numSlots;

        public string SteamLobbyId { get; private set; }
        public string MatchId { get; private set; }
        public string JoinLink { get; private set; }
        public string Speed { get; private set; }
        public string GameType { get; private set; }
        public string Map { get; private set; }

        public ICommand JoinGameCommand { get; }
        public ICommand CopyLobbyLinkCommand { get; }

        [ObservableProperty]
        private bool _isNew;

        public DateTime AddedAt { get; set; }

        public List<Player> Players { get; } = new List<Player>();

        public async Task JoinGame()
        {
            Process.Start("open", JoinLink);

            //await Windows.System.Launcher.LaunchUriAsync(new Uri(JoinLink));
        }

        public async Task CopyJoinLinkToClipboard()
        {
            await App.Clipboard.SetTextAsync(JoinLink);
        }

        public bool ContainsPlayer(string playerQuery)
        {
            return Players.Any(x =>
                x.Name.ToLowerInvariant() == playerQuery.ToLowerInvariant() ||
                x.SteamProfileId == playerQuery);
        }

        public static Lobby Create(LobbyDto dto)
        {
            var lobby = new Lobby()
            {
                SteamLobbyId = dto.SteamLobbyId,
                MatchId = dto.MatchId,
                Name = dto.Name,
                NumPlayers = dto.Players.Count,
                NumSlots = dto.NumSlots,
                Speed = dto.Speed,
                GameType = dto.GameType,
                Map = dto.MapType,
            };
            lobby.Players.AddRange(dto.Players.OrderBy(x => x.Slot).Take(dto.NumSlots).Select(x => Player.Create(x)));
            if (AppSettings.JoinLinkType == AppSettings.JoinLink.Aoe2de)
            {
                lobby.JoinLink = $"aoe2de://0/{lobby.MatchId}";
            }
            else
            {
                lobby.JoinLink = $"steam://joinlobby/813780/{lobby.SteamLobbyId}";
            }
            return lobby;
        }
    }
}
