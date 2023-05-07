using AoE2DELobbyBrowser.Services;
using DynamicData.Binding;

namespace AoE2DELobbyBrowser.Models
{
    public class Friend : AbstractNotifyPropertyChanged
    {
        public string SteamId { get; set; }
        public string Name { get; private set; }
        public string StreamProfileUrl => $"https://steamcommunity.com/profiles/{SteamId}";
        public string Country { get; private set; }

        private Lobby _lobby;
        public Lobby Lobby
        {
            get => _lobby;
            set => SetAndRaise(ref _lobby, value);
        }

        public static Friend Create(SteamPlayerDto dto)
        {
            return new Friend
            {
                Country = dto.LocCountryCode,
                Name = dto.PersonaName,
                SteamId = dto.SteamId,
            };
        }
    }
}
