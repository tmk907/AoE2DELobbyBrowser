using AoE2DELobbyBrowserAvalonia.Api;
using AoE2DELobbyBrowserAvalonia.Services;
using Avalonia.Input.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AoE2DELobbyBrowserAvalonia.Models
{
    public partial class LobbyVM : ObservableObject
    {
        public LobbyVM()
        {
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

        [ObservableProperty]
        private bool _isNew;

        public DateTime AddedAt { get; set; }

        public List<PlayerVM> Players { get; } = new List<PlayerVM>();

        [RelayCommand]
        private async Task JoinGame()
        {
            var launcher = Ioc.Default.GetRequiredService<ILauncherService>();
            await launcher.LauchUriAsync(new Uri(JoinLink));
        }

        [RelayCommand]
        private async Task CopyLobbyLink()
        {
            var clipboard = Ioc.Default.GetRequiredService<IClipboard>();
            await clipboard.SetTextAsync(JoinLink);
        }

        public bool ContainsPlayer(string playerQuery)
        {
            return Players.Any(x =>
                x.Name.ToLowerInvariant() == playerQuery.ToLowerInvariant() ||
                x.SteamProfileId == playerQuery);
        }

        public static LobbyVM Create(LobbyDto dto)
        {
            var lobby = new LobbyVM()
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
            lobby.Players.AddRange(dto.Players.OrderBy(x => x.Slot).Take(dto.NumSlots).Select(x => PlayerVM.Create(x)));
            if (AppSettings.JoinLinkType == JoinLinkEnum.Aoe2de)
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
