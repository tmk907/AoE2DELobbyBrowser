using System.Text.Json;
using Windows.Storage;

namespace AoE2DELobbyNotifications
{
    internal class AppSettingsService
    {
        private ApplicationDataContainer _localSettings;

        public AppSettingsService()
        {
            _localSettings = ApplicationData.Current.LocalSettings;
        }

        public T Get<T>(string key, T defaultValue)
        {
            if(_localSettings.Values.TryGetValue(key, out object serialized))
            {
                var data = JsonSerializer.Deserialize<T>(serialized as string);
                return data;
            }
            else
            {
                Save(key, defaultValue);
                return defaultValue;
            }
        }

        public void Save<T>(string key, T data)
        {
            var serialized = JsonSerializer.Serialize(data);
            _localSettings.Values[key]=serialized;
        }
    }
}
