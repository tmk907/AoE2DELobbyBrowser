using System.Text.Json;


namespace AoE2DELobbyBrowserAvalonia.Services
{
    public interface IObjectSerializer
    {
        T Deserialize<T>(string value);
        string Serialize<T>(T value);
    }

    class AppDataJsonSerializer : IObjectSerializer
    {
        public T Deserialize<T>(string value)
        {
            return JsonSerializer.Deserialize<T>(value);
        }

        public string Serialize<T>(T value)
        {
            return JsonSerializer.Serialize(value);
        }
    }
}
