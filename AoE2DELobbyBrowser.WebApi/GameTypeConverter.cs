namespace AoE2DELobbyBrowser.WebApi
{
    public class GameTypeConverter
    {
        private const string Unknown = "Unknown";

        private static Dictionary<int, string> _map = new Dictionary<int, string>()
        {
            { 0, "Random Map" },
            { 1, "Regicide" },
            { 2, "Death Match" },
            { 3, "Scenario" },
            { 6, "King of the Hill" },
            { 7, "Wonder Race" },
            { 8, "Defend the Wonder" },
            { 9, "Turbo Random Map" },
            { 10, "Capture the Relic" },
            { 11, "Sudden Death" },
            { 12, "Battle Royale" },
            { 13, "Empire Wars" },
            { 15, "Co-Op Campaign" },
        };

        public static string ToName(int? gameType)
        {
            if (gameType.HasValue)
            {
                if (_map.ContainsKey(gameType.Value)) return _map[gameType.Value];
            }
            return Unknown;
        }
    }
}
