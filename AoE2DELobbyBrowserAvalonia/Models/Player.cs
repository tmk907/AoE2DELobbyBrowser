using AoE2DELobbyBrowserAvalonia.Api;
using AoE2DELobbyBrowserAvalonia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System;

namespace AoE2DELobbyBrowserAvalonia.Models
{
    public partial class Player : ObservableObject
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
            CountryName = Ioc.Default.GetRequiredService<CountryService>().GetCountryName(Country);
        }

        public void UpdateCountry(string country)
        {
            if (country == null) return;
            Country = country;
            CountryName = Ioc.Default.GetRequiredService<CountryService>().GetCountryName(Country);
            OnPropertyChanged(nameof(Country));
            OnPropertyChanged(nameof(CountryName));
        }

        [RelayCommand]
        private async Task OpenUrl(string url)
        {
            var launcher = Ioc.Default.GetRequiredService<ILauncherService>();
            await launcher.LauchUriAsync(new Uri(url));
        }

        public static Player Create(PlayerDto dto)
        {
            return new Player(dto.Name, dto.SteamProfileId, dto.Country);
        }
    }
}
