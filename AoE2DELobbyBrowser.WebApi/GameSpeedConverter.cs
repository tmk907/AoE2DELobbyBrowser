namespace AoE2DELobbyBrowser.WebApi
{
    public class GameSpeedConverter
    {
        private const string Unknown = "Unknown";

        private static Dictionary<int, string> _map = new Dictionary<int, string>()
        {
            { 0, "Slow" },
            { 1, "Casual" },
            { 2, "Normal" },
            { 3, "Fast" },
        };

        public static string ToName(int? gameSpeed)
        {
            if (gameSpeed.HasValue)
            {
                if (_map.ContainsKey(gameSpeed.Value)) return _map[gameSpeed.Value];
            }
            return Unknown;
        }
    }
}
