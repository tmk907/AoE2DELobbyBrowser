using System.Text.Json.Serialization;

namespace AoE2DELobbyBrowser.WebApi
{
    public class LobbyDto
    {
        [JsonPropertyName("lobby_id")]
        public string LobbyId { get; set; }

        [JsonPropertyName("match_id")]
        public string MatchId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("num_players")]
        public int NumPlayers { get; set; }

        [JsonPropertyName("num_slots")]
        public int NumSlots { get; set; }

        [JsonPropertyName("game_type")]
        public string GameType { get; set; }

        [JsonPropertyName("map_type")]
        public string MapType { get; set; }

        [JsonPropertyName("speed")]
        public string Speed { get; set; }

        [JsonPropertyName("opened")]
        public int? Opened { get; set; }

        [JsonPropertyName("players")]
        public List<PlayerDto> Players { get; set; }

        public static LobbyDto Create(Aoe2net.LobbyDto dto)
        {
            return new LobbyDto
            {
                GameType = GameTypeConverter.ToName(dto.GameType),
                LobbyId = dto.LobbyId,
                MatchId = dto.MatchId,
                MapType = MapTypeConverter.ToName(dto.MapType),
                Name = dto.Name,
                NumPlayers = dto.NumPlayers,
                NumSlots = dto.NumSlots,
                Opened = dto.Opened,
                Players = dto.Players.Select(x => PlayerDto.Create(x)).ToList(),
                Speed = GameSpeedConverter.ToName(dto.Speed)
            };
        }
    }
}
