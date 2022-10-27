using System.Text.Json.Serialization;

namespace AoE2DELobbyBrowser.WebApi.v2.Reliclink
{
    public class Avatar
    {
        [JsonPropertyName("profile_id")]
        public int ProfileId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("alias")]
        public string Alias { get; set; }

        [JsonPropertyName("personal_statgroup_id")]
        public int PersonalStatgroupId { get; set; }

        [JsonPropertyName("xp")]
        public int Xp { get; set; }

        [JsonPropertyName("level")]
        public int Level { get; set; }

        [JsonPropertyName("leaderboardregion_id")]
        public int LeaderboardregionId { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }
    }
}
