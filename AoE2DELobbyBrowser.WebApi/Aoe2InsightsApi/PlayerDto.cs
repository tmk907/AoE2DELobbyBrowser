using System.Text.Json.Serialization;

namespace AoE2DELobbyBrowser.WebApi.Aoe2InsightsApi
{
    public class PlayerDto
    {
        [JsonPropertyName("color")]
        public int? Color { get; set; }

        [JsonPropertyName("team")]
        public int? Team { get; set; }

        [JsonPropertyName("slot")]
        public int Slot { get; set; }

        [JsonPropertyName("slot_type")]
        public int? SlotType { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("user_id")]
        public int? UserId { get; set; }

        [JsonPropertyName("avatar")]
        public string? Avatar { get; set; }

        [JsonPropertyName("elo_1v1")]
        public int? Elo1v1 { get; set; }

        [JsonPropertyName("elo_team")]
        public int? EloTeam { get; set; }
    }
}
