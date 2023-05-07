using AoE2DELobbyBrowser.Api;

namespace AoE2DELobbyBrowser.Models
{
    public class Player
    {
        public string SteamProfileId { get; private set; }
        public string Name { get; private set; }
        public string SteamProfileUrl { get; private set; }
        public string Country { get; private set; }
        public static Player Create(PlayerDto dto)
        {
            return new Player
            {
                Name = dto.Name,
                SteamProfileId = dto.SteamProfileId,
                SteamProfileUrl = $"https://steamcommunity.com/profiles/{dto.SteamProfileId}",
                Country = dto.Country,
            };
        }
    }
}
