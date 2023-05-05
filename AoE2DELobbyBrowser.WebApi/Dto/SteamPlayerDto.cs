using System.Text.Json.Serialization;

namespace AoE2DELobbyBrowser.WebApi.Dto
{
    public class SteamPlayerDto
    {
        [JsonPropertyName("steamid")]
        public string Steamid { get; set; }

        [JsonPropertyName("personaname")]
        public string Personaname { get; set; }

        [JsonPropertyName("profileurl")]
        public string Profileurl { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }

        [JsonPropertyName("loccountrycode")]
        public string Loccountrycode { get; set; }
    }
}
