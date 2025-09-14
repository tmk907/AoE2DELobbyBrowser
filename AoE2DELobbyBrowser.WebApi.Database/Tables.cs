namespace AoE2DELobbyBrowser.WebApi.Database
{
    public class LobbiesTable
    {
        public int MatchId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<int> PlayerIds { get; set; }
        public string Name { get; set; }
        public string GameType { get; set; }
        public string MapType { get; set; }
        public string Speed { get; set; }
        public string Dataset { get; set; }
        public string ModId { get; set; }
        public string Scenario { get; set; }
        public int IsObservable { get; set; }
    }

    public class MatchesTable
    {
        public int MatchId { get; set; }
        public string Json { get; set; }
    }
}
