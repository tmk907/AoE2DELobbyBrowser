using System.Text.Json.Serialization;

namespace AoE2DELobbyBrowser.Services
{
    public class SteamPlayerDto
    {
        [JsonPropertyName("steamid")]
        public string SteamId { get; set; }

        [JsonPropertyName("personaname")]
        public string PersonaName { get; set; }

        [JsonPropertyName("profileurl")]
        public string ProfileUrl { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }

        [JsonPropertyName("loccountrycode")]
        public string LocCountryCode { get; set; }
    }
}
