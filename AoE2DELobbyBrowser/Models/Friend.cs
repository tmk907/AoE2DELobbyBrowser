using AoE2DELobbyBrowser.Api;
using DynamicData.Binding;

namespace AoE2DELobbyBrowser.Models
{
    public class Friend : AbstractNotifyPropertyChanged
    {
        public Player Player { get; set; }

        private string _lobbyName;
        public string LobbyName
        {
            get => _lobbyName;
            set => SetAndRaise(ref _lobbyName, value);
        }

        private Lobby _lobby;
        public Lobby Lobby
        {
            get => _lobby;
            set => SetAndRaise(ref _lobby, value);
        }

        public Friend(string name, string profileId, string country)
        {
            Player = new Player(name, profileId, country);
        }

        public static Friend Create(SteamPlayerDto dto)
        {
            return new Friend(dto.PersonaName, dto.SteamId, dto.LocCountryCode);
        }
    }
}
