using AoE2DELobbyBrowserAvalonia.Api;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AoE2DELobbyBrowserAvalonia.Models
{
    public class Player : ObservableObject
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
            if (country == null) return;
            Country = country;
            CountryName = App.CountryService.GetCountryName(Country);
            OnPropertyChanged(nameof(Country));
            OnPropertyChanged(nameof(CountryName));
        }

        public static Player Create(PlayerDto dto)
        {
            return new Player(dto.Name, dto.SteamProfileId, dto.Country);
        }
    }
}
