using System.Collections.Generic;
using System.Linq;

namespace AoE2DELobbyBrowser
{
    public class GameType 
    {
        public const string All = "All";
        public const string Unknown = "Unknown";
        public const string Scenario = "Scenario";

        private Dictionary<int, string> _map = new Dictionary<int, string>()
        {
            { -1, All},
            { 0, "Random Map" },
            { 1, "Regicide" },
            { 2, "Death Match" },
            { 3, Scenario },
            { 6, "King of the Hill" },
            { 7, "Wonder Race" },
            { 8, "Defend the Wonder" },
            { 9, "Turbo Random Map" },
            { 10, "Capture the Relic" },
            { 11, "Sudden Death" },
            { 12, "Battle Royale" },
            { 13, "Empire Wars" },
            { 15, "Co-Op Campaign" },
            { -2, Unknown },
        };

        public List<string> GetAll() => _map.Select(kv => kv.Value).ToList();

        public string GetById(int id)
        {
            if (_map.ContainsKey(id)) return _map[id];
            return Unknown;
        }
    }
}
