using AoE2DELobbyBrowserAvalonia;
using AoE2DELobbyBrowserAvalonia.Models;
using System.Linq;
using System.Text.Json;

namespace AoE2DELobbyBrowser.Services
{
    internal class AppSettingsService
    {
        private readonly IAppSettings _appSettings;

        public AppSettingsService(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public LobbySettings GetLobbySettings()
        {
            if (_appSettings.LobbySettings == null)
            {
                _appSettings.LobbySettings = JsonSerializer.Serialize(new LobbySettings
                {
                    Interval = 10,
                    IsAutoRefreshEnabled = true,
                    Query = "",
                    Exclude = "",
                    SelectedGameSpeed = new GameSpeed().GetAll().First(),
                    SelectedGameType = new GameType().GetAll().First(),
                    SelectedMapType = new MapType().GetAll().First(),
                    ShowNotifications = false,
                });
            }
            return JsonSerializer.Deserialize<LobbySettings>(_appSettings.LobbySettings);
        }

        public void SaveLobbySettings(LobbySettings settings)
        {
            _appSettings.LobbySettings = JsonSerializer.Serialize(settings);
        }
    }
}
