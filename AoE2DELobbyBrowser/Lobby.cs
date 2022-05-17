using AoE2DELobbyBrowser.Api;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace AoE2DELobbyBrowser
{
    public class Lobby: AbstractNotifyPropertyChanged
    {
        public Lobby()
        {
            JoinGameCommand = ReactiveCommand.CreateFromTask(JoinGame);
            CopyLobbyLinkCommand = ReactiveCommand.Create(() => CopyJoinLinkToClipboard());
            AddedAt = DateTime.Now;
        }

        public string Name { get; set; }

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

        public string LobbyId { get; set; }
        public string MatchId { get; set; }
        public string JoinLink { get; set; }
        public string Speed { get; set; }
        public string GameType { get; set; }
        public string Map { get; set; }
        public DateTime OpenedAt { get; set; }
        public bool IsUnknownOpenedAt { get; set; }

        public ReactiveCommand<Unit, Unit> JoinGameCommand { get; }
        public ReactiveCommand<Unit,Unit> CopyLobbyLinkCommand { get; }

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
                LobbyId = dto.LobbyId,
                MatchId = dto.MatchId,
                Name = dto.Name,
                NumPlayers = dto.NumPlayers,
                NumSlots = dto.NumSlots,
                Speed = dto.Speed,
                GameType = dto.GameType,
                Map = dto.MapType,
                OpenedAt = DateTimeOffset.FromUnixTimeSeconds(dto.Opened ?? 0).ToLocalTime().DateTime,
                IsUnknownOpenedAt = (dto.Opened ?? 0) == 0
            };
            lobby.Players.AddRange(dto.Players.OrderBy(x => x.Slot).Take(dto.NumSlots).Select(x => Player.Create(x)));
            if (AppSettings.JoinLinkType == AppSettings.JoinLink.Aoe2de)
            {
                lobby.JoinLink = $"aoe2de://0/{lobby.MatchId}";
            }
            else
            {
                lobby.JoinLink = $"steam://joinlobby/813780/{lobby.LobbyId}";
            }
            return lobby;
        }

        public static Lobby Create(Api.Aoe2net.LobbyDto dto)
        {
            var gameTypes = new GameType();
            var gameSpeed = new GameSpeed();
            var mapTypes = new MapType();

            var lobby = new Lobby()
            {
                LobbyId = dto.LobbyId,
                Name = dto.Name,
                NumPlayers = dto.NumPlayers,
                NumSlots = dto.NumSlots,
                Speed = gameSpeed.GetById(dto.Speed.GetValueOrDefault(-2)),
                GameType = gameTypes.GetById(dto.GameType.GetValueOrDefault(-2)),
                Map = mapTypes.GetById(dto.MapType.GetValueOrDefault(-2)),
                OpenedAt = DateTimeOffset.FromUnixTimeSeconds(dto.Opened ?? 0).ToLocalTime().DateTime,
                IsUnknownOpenedAt = (dto.Opened ?? 0) == 0
            };
            lobby.Players.AddRange(dto.Players.OrderBy(x => x.Slot).Take(dto.NumSlots).Select(x => Player.Create(x)));
            if (AppSettings.JoinLinkType == AppSettings.JoinLink.Aoe2de)
            {
                lobby.JoinLink = $"aoe2de://0/{lobby.MatchId}";
            }
            else
            {
                lobby.JoinLink = $"steam://joinlobby/813780/{lobby.LobbyId}";
            }
            return lobby;
        }
    }
}
