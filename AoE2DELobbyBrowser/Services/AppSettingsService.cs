using AoE2DELobbyBrowser.Models;
using System;
using System.Linq;
using System.Text.Json;
using Windows.Storage;

namespace AoE2DELobbyBrowser.Services
{
    internal class AppSettingsService
    {
        private ApplicationDataContainer _localSettings;

        public AppSettingsService()
        {
            _localSettings = ApplicationData.Current.LocalSettings;
        }

        public T Get<T>(string key, Func<T> createDefaultValue)
        {
            if (_localSettings.Values.TryGetValue(key, out object serialized))
            {
                var data = JsonSerializer.Deserialize<T>(serialized as string);
                return data;
            }
            else
            {
                var defaultValue = createDefaultValue();
                Save(key, defaultValue);
                return defaultValue;
            }
        }

        public void Save<T>(string key, T data)
        {
            var serialized = JsonSerializer.Serialize(data);
            _localSettings.Values[key] = serialized;
        }

        public LobbySettings GetLobbySettings()
        {
            return Get("lobby-settings", () => new LobbySettings
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

        public void SaveLobbySettings(LobbySettings settings)
        {
            Save("lobby-settings", settings);
        }
    }
}
