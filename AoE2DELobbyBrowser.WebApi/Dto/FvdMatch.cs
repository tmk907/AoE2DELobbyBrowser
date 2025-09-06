namespace AoE2DELobbyBrowser.WebApi.Reliclink
{
    public class FvdMatch
    {
        public int MatchId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<int> PlayerIds { get; set; }

        public FvdMatch(int matchId, IEnumerable<int> playerIds)
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
}
