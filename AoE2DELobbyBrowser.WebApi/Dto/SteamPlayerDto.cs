using System.Text.Json.Serialization;

namespace AoE2DELobbyBrowser.WebApi.Dto
{
    public class SteamPlayerDto
    {
        [JsonPropertyName("steamid")]
        public string Steamid { get; set; }

        [JsonPropertyName("personaname")]
        public string Name { get; set; }

        [JsonPropertyName("profileurl")]
        public string ProfileUrl { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }

        [JsonPropertyName("loccountrycode")]
        public string CountryCode { get; set; }

        [JsonPropertyName("personastate")]
        public int Status { get; set; }
    }
}
