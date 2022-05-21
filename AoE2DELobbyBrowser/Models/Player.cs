using AoE2DELobbyBrowser.Api;

namespace AoE2DELobbyBrowser.Models
{
    public class Player
    {
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Country { get; set; }
        public string Rating { get; set; }
        public string Games { get; set; }
        public string Wins { get; set; }
        public string Streak { get; set; }
        public string Drops { get; set; }

        public static Player Create(PlayerDto dto)
        {
            return new Player
            {
                Name = dto.Name,
                Avatar = dto.Avatar,
                Country = dto.Country,
                Rating = FormatValue(dto.Rating),
                Games = FormatValue(dto.Games),
                Wins = FormatValue(dto.Wins),
                Streak = FormatValue(dto.Streak),
                Drops = FormatValue(dto.Drops)
            };
        }

        private static string FormatValue(int? value)
        {
            if (value.HasValue) return value.ToString();
            return "";
        }
    }
}
