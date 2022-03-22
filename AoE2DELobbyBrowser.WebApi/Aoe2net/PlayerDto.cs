using System.Text.Json.Serialization;

namespace AoE2DELobbyBrowser.WebApi.Aoe2net
{
    public class PlayerDto
    {
        [JsonPropertyName("profile_id")]
        public int? ProfileId { get; set; }

        [JsonPropertyName("steam_id")]
        public string SteamId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("clan")]
        public string Clan { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("slot")]
        public int Slot { get; set; }

        [JsonPropertyName("slot_type")]
        public int SlotType { get; set; }

        [JsonPropertyName("rating")]
        public int? Rating { get; set; }

        [JsonPropertyName("rating_change")]
        public object RatingChange { get; set; }

        [JsonPropertyName("games")]
        public int? Games { get; set; }

        [JsonPropertyName("wins")]
        public int? Wins { get; set; }

        [JsonPropertyName("streak")]
        public int? Streak { get; set; }

        [JsonPropertyName("drops")]
        public int? Drops { get; set; }

        [JsonPropertyName("color")]
        public object Color { get; set; }

        [JsonPropertyName("team")]
        public object Team { get; set; }

        [JsonPropertyName("civ")]
        public object Civ { get; set; }

        [JsonPropertyName("civ_alpha")]
        public object CivAlpha { get; set; }

        [JsonPropertyName("won")]
        public object Won { get; set; }
    }
}
