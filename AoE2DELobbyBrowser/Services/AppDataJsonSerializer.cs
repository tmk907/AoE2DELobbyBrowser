using System.Text.Json;


namespace AoE2DELobbyBrowser.Services
{
    class AppDataJsonSerializer : CommunityToolkit.Common.Helpers.IObjectSerializer
    {
        public T Deserialize<T>(string value)
        {
            return JsonSerializer.Deserialize<T>(value);
        }

        string CommunityToolkit.Common.Helpers.IObjectSerializer.Serialize<T>(T value)
        {
            return JsonSerializer.Serialize(value);
        }
    }
}
