using System.Text.Json.Serialization;

namespace AoE2DELobbyBrowser.WebApi.Aoe2netWebsocket
{
    public class WebsocketMessage<T>
    {
        [JsonPropertyName("data")]
        public T Data { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    public class LobbiesWebsocketMessage : WebsocketMessage<List<LobbyDto>>
    {
    }

    public class PingWebsocketMessage : WebsocketMessage<long>
    {
    }

    public class SubscribeWebsocketMessage
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("subscribe")]
        public List<int> Subscribe { get; set; }
    }
}
