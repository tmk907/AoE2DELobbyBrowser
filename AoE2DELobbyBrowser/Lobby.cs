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
        }

        public string Name { get; set; }
        public int NumPlayers { get; set; }
        public int NumSlots { get; set; }
        public string LobbyId { get; set; }
        public string JoinLink => $"steam://joinlobby/813780/{LobbyId}";
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
            return lobby;
        }
    }
}
