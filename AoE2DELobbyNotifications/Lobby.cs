using AoE2DELobbyNotifications.Api;
using ReactiveUI;
using System;
using System.Reactive;
using System.Threading.Tasks;

namespace AoE2DELobbyNotifications
{
    public class Lobby
    {
        public Lobby()
        {
            JoinGameCommand = ReactiveCommand.CreateFromTask(JoinGame);
        }

        public string Name { get; set; }
        public int NumPlayers { get; set; }
        public int NumSlots { get; set; }
        public string LobbyId { get; set; }
        public string JoinLink => $"steam://joinlobby/813780/{LobbyId}";
        public string Speed { get; set; }
        public string GameType { get; set; }

        public ReactiveCommand<Unit, Unit> JoinGameCommand { get; }

        public bool IsNew { get; set; }

        public async Task JoinGame()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(JoinLink));
        }

        public static Lobby Create(LobbyDto dto)
        {
            var gameTypes = new GameType();
            var gameSpeed = new GameSpeed();
            return new Lobby()
            {
                LobbyId = dto.LobbyId,
                Name = dto.Name,
                NumPlayers = dto.NumPlayers,
                NumSlots = dto.NumSlots,
                Speed = gameSpeed.GetById(dto.Speed),
                GameType = gameTypes.GetById(dto.GameType)
            };
        }
    }
}
