using System.Text.Json.Serialization;

namespace AoE2DELobbyBrowser.WebApi.Dto
{
    public class PlayerDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("profile_id")]
        public string ProfileId { get; set; }

        [JsonPropertyName("steam_profile_id")]
        public string SteamProfileId { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("slot")]
        public int Slot { get; set; }
    }
}
