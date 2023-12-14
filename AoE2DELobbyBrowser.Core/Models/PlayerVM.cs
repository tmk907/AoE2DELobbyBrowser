using AoE2DELobbyBrowser.Core.Api;
using AoE2DELobbyBrowser.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System;

namespace AoE2DELobbyBrowser.Core.Models
{
    public partial class PlayerVM : ObservableObject
    {
        public string SteamProfileId { get; private set; }
        public string Name { get; private set; }
        public string SteamProfileUrl { get; private set; }
        public string Country { get; private set; }
        public string CountryName { get; private set; }

        public PlayerVM(string name, string profileId, string countryCode)
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

        public static PlayerVM Create(PlayerDto dto)
        {
            return new PlayerVM(dto.Name, dto.SteamProfileId, dto.Country);
        }
    }
}
