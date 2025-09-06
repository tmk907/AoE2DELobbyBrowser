namespace AoE2DELobbyBrowser.WebApi.Reliclink
{
    class FvdMatch
    {
        public int MatchId { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<int> PlayerIds { get; set; }

        public FvdMatch(int matchId, IEnumerable<int> playerIds)
        {
            MatchId = matchId;
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
