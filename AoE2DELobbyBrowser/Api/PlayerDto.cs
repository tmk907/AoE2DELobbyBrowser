using System.Text.Json.Serialization;

namespace AoE2DELobbyBrowser.Api
{
    public class PlayerDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("avatar")]
        public string? Avatar { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("slot")]
        public int Slot { get; set; }

        [JsonPropertyName("rating")]
        public int? Rating { get; set; }

        [JsonPropertyName("games")]
        public int? Games { get; set; }

        [JsonPropertyName("wins")]
        public int? Wins { get; set; }

        [JsonPropertyName("streak")]
        public int? Streak { get; set; }

        [JsonPropertyName("drops")]
        public int? Drops { get; set; }
    }
}
