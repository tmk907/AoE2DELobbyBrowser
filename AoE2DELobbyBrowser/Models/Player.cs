using AoE2DELobbyBrowser.Api;

namespace AoE2DELobbyBrowser.Models
{
    public class Player
    {
        public string SteamProfileId { get; private set; }
        public string Name { get; private set; }
        public string SteamProfileUrl { get; private set; }
        public string Country { get; private set; }
        public string CountryName { get; private set; }

        public Player(string name, string profileId, string countryCode)
        {
            Name = name;
            SteamProfileId = profileId;
            SteamProfileUrl = $"https://steamcommunity.com/profiles/{SteamProfileId}";
            Country = countryCode;
            CountryName = App.CountryService.GetCountryName(Country);
        }

        public void UpdateCountry(string country)
        {
            Country = country;
            CountryName = App.CountryService.GetCountryName(Country);
        }

        public static Player Create(PlayerDto dto)
        {
            return new Player(dto.Name, dto.SteamProfileId, dto.Country);
        }
    }
}
