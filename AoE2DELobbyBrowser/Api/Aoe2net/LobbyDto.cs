using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AoE2DELobbyBrowser.Api.Aoe2net
{
    public class LobbyDto
    {
        [JsonPropertyName("match_id")]
        public string MatchId { get; set; }

        [JsonPropertyName("lobby_id")]
        public string LobbyId { get; set; }

        [JsonPropertyName("match_uuid")]
        public string MatchUuid { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("num_players")]
        public int NumPlayers { get; set; }

        [JsonPropertyName("num_slots")]
        public int NumSlots { get; set; }

        [JsonPropertyName("average_rating")]
        public int? AverageRating { get; set; }

        [JsonPropertyName("cheats")]
        public bool? Cheats { get; set; }

        [JsonPropertyName("full_tech_tree")]
        public bool? FullTechTree { get; set; }

        [JsonPropertyName("ending_age")]
        public int? EndingAge { get; set; }

        [JsonPropertyName("expansion")]
        public object Expansion { get; set; }

        [JsonPropertyName("game_type")]
        public int? GameType { get; set; }

        [JsonPropertyName("has_custom_content")]
        public object HasCustomContent { get; set; }

        [JsonPropertyName("has_password")]
        public bool? HasPassword { get; set; }

        [JsonPropertyName("lock_speed")]
        public bool? LockSpeed { get; set; }

        [JsonPropertyName("lock_teams")]
        public bool? LockTeams { get; set; }

        [JsonPropertyName("map_size")]
        public int? MapSize { get; set; }

        [JsonPropertyName("map_type")]
        public int? MapType { get; set; }

        [JsonPropertyName("pop")]
        public int? Pop { get; set; }

        [JsonPropertyName("ranked")]
        public bool? Ranked { get; set; }

        [JsonPropertyName("leaderboard_id")]
        public int? LeaderboardId { get; set; }

        [JsonPropertyName("rating_type")]
        public int? RatingType { get; set; }

        [JsonPropertyName("resources")]
        public int? Resources { get; set; }

        [JsonPropertyName("rms")]
        public string Rms { get; set; }

        [JsonPropertyName("scenario")]
        public string Scenario { get; set; }

        [JsonPropertyName("server")]
        public string Server { get; set; }

        [JsonPropertyName("shared_exploration")]
        public bool? SharedExploration { get; set; }

        [JsonPropertyName("speed")]
        public int? Speed { get; set; }

        [JsonPropertyName("starting_age")]
        public int? StartingAge { get; set; }

        [JsonPropertyName("team_together")]
        public bool? TeamTogether { get; set; }

        [JsonPropertyName("team_positions")]
        public bool? TeamPositions { get; set; }

        [JsonPropertyName("treaty_length")]
        public int? TreatyLength { get; set; }

        [JsonPropertyName("turbo")]
        public bool? Turbo { get; set; }

        [JsonPropertyName("victory")]
        public int? Victory { get; set; }

        [JsonPropertyName("victory_time")]
        public int? VictoryTime { get; set; }

        [JsonPropertyName("visibility")]
        public int? Visibility { get; set; }

        [JsonPropertyName("opened")]
        public int? Opened { get; set; }

        [JsonPropertyName("started")]
        public object Started { get; set; }

        [JsonPropertyName("finished")]
        public object Finished { get; set; }

        [JsonPropertyName("players")]
        public List<PlayerDto> Players { get; set; }
    }
}
