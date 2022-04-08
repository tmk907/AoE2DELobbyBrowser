using System.Text.Json.Serialization;

namespace AoE2DELobbyBrowser.WebApi
{
    public class PlayerDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        //[JsonPropertyName("avatar")]
        //public string? Avatar { get; set; }

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

        public static PlayerDto Create(Aoe2net.PlayerDto dto)
        {
            return new PlayerDto
            {
                Country = dto.Country,
                Drops = dto.Drops,
                Games = dto.Games,
                Name = dto.Name,
                Rating = dto.Rating,
                Slot = dto.Slot,
                Streak = dto.Streak,
                Wins = dto.Wins
            };
        }
    }
}
