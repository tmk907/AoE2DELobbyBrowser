using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AoE2DELobbyBrowser.Core.Api
{
    public class LobbyDto
    {
        [JsonPropertyName("steam_lobby_id")]
        public string SteamLobbyId { get; set; }

        [JsonPropertyName("match_id")]
        public string MatchId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("game_type")]
        public string GameType { get; set; }

        [JsonPropertyName("map_type")]
        public string MapType { get; set; }

        [JsonPropertyName("speed")]
        public string Speed { get; set; }

        [JsonPropertyName("num_slots")]
        public int NumSlots { get; set; }

        [JsonPropertyName("players")]
        public List<PlayerDto> Players { get; set; }
    }
}
