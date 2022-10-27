using AoE2DELobbyBrowser.Api;
using AoE2DELobbyBrowser.Services;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace AoE2DELobbyBrowser.Models
{
    public class Lobby : AbstractNotifyPropertyChanged
    {
        public Lobby()
        {
            JoinGameCommand = ReactiveCommand.CreateFromTask(JoinGame);
            CopyLobbyLinkCommand = ReactiveCommand.Create(() => CopyJoinLinkToClipboard());
            AddedAt = DateTime.Now;
        }

        public string Name { get; private set; }

        private int _numPlayers;
        public int NumPlayers
        {
            get => _numPlayers;
            set => SetAndRaise(ref _numPlayers, value);
        }

        private int _numSlots;
        public int NumSlots
        {
            get => _numSlots;
            set => SetAndRaise(ref _numSlots, value);
        }

        public string SteamLobbyId { get; private set; }
        public string MatchId { get; private set; }
        public string JoinLink { get; private set; }
        public string Speed { get; private set; }
        public string GameType { get; private set; }
        public string Map { get; private set; }

        public ReactiveCommand<Unit, Unit> JoinGameCommand { get; }
        public ReactiveCommand<Unit, Unit> CopyLobbyLinkCommand { get; }

        private bool _isNew;
        public bool IsNew
        {
            get => _isNew;
            set => SetAndRaise(ref _isNew, value);
        }

        public DateTime AddedAt { get; set; }

        public List<Player> Players { get; } = new List<Player>();

        public async Task JoinGame()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(JoinLink));
        }

        public void CopyJoinLinkToClipboard()
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText(JoinLink);
            Clipboard.SetContent(dataPackage);
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
