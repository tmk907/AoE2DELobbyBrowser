using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace AoE2DELobbyBrowser.WebApi.Reliclink
{
    public class CachedLobby
    {
        [JsonPropertyName("match_id")]
        public int MatchId { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("player_ids")]
        public List<int> PlayerIds { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("game_type")]
        public string GameType { get; set; }

        [JsonPropertyName("map_type")]
        public string MapType { get; set; }

        [JsonPropertyName("speed")]
        public string Speed { get; set; }

        [JsonPropertyName("dataset")]
        public string Dataset { get; set; }

        [JsonPropertyName("mod_id")]
        public string ModId { get; set; }

        [JsonPropertyName("scenario")]
        public string Scenario { get; set; }

        [JsonPropertyName("isobservable")]
        public bool IsObservable { get; set; }

        public CachedLobby(int matchId, IEnumerable<int> playerIds)
        {
            MatchId = matchId;
            CreatedAt = DateTime.UtcNow;
            PlayerIds = playerIds.ToList();
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePlayers(IEnumerable<int> playerIds)
        {
            PlayerIds = playerIds.ToList();
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public class CachedLobbiesQuery
    {
        [FromQuery]
        public string? Name { get; set; }

        [FromQuery]
        public string? GameType { get; set; }

        [FromQuery]
        public string? MapType { get; set; }

        [FromQuery]
        public string? Speed { get; set; }

        [FromQuery]
        public string? Dataset { get; set; }

        [FromQuery]
        public string? ModId { get; set; }

        [FromQuery]
        public string? Scenario { get; set; }

        [FromQuery]
        public int? PlayerId { get; set; }

        [FromQuery]
        public int? IsObservable { get; set; }
    }
}
