using AoE2DELobbyBrowser.Api;

namespace AoE2DELobbyBrowser.Models
{
    public class Player
    {
        public string Name { get; private set; }
        public string StreamProfileUrl { get; private set; }
        public string Country { get; private set; }
        public static Player Create(PlayerDto dto)
        {
            return new Player
            {
                Name = dto.Name,
                StreamProfileUrl = $"https://steamcommunity.com/profiles/{dto.SteamProfileId}",
                Country = dto.Country,
            };
        }
    }
}
