﻿using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AoE2DELobbyBrowser.Api
{
    public class LobbyDto
    {
        [JsonPropertyName("lobby_id")]
        public string LobbyId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("num_players")]
        public int NumPlayers { get; set; }

        [JsonPropertyName("num_slots")]
        public int NumSlots { get; set; }

        [JsonPropertyName("game_type")]
        public int? GameType { get; set; }

        [JsonPropertyName("map_type")]
        public int? MapType { get; set; }
       
        [JsonPropertyName("scenario")]
        public string Scenario { get; set; }

        [JsonPropertyName("speed")]
        public int? Speed { get; set; }

        [JsonPropertyName("opened")]
        public int? Opened { get; set; }

        [JsonPropertyName("players")]
        public List<PlayerDto> Players { get; set; }
    }
}
