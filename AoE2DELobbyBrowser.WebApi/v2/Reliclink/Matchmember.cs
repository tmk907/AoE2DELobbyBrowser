using System.Text.Json.Serialization;

namespace AoE2DELobbyBrowser.WebApi.v2.Reliclink
{
    public class Matchmember
    {
        [JsonPropertyName("match_id")]
        public int MatchId { get; set; }

        [JsonPropertyName("profile_id")]
        public int ProfileId { get; set; }

        [JsonPropertyName("ranking")]
        public int Ranking { get; set; }

        [JsonPropertyName("statgroup_id")]
        public int StatgroupId { get; set; }

        [JsonPropertyName("race_id")]
        public int RaceId { get; set; }

        [JsonPropertyName("teamid")]
        public int Teamid { get; set; }
    }
}
