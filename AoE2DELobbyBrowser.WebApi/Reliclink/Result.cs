using System.Text.Json.Serialization;

namespace AoE2DELobbyBrowser.WebApi.Reliclink
{
    public class Result
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
