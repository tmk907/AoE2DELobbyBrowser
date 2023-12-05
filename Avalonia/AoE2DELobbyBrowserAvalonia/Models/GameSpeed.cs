using System.Collections.Generic;
using System.Linq;

namespace AoE2DELobbyBrowserAvalonia.Models
{
    public class GameSpeed
    {
        public const string All = "All";
        public const string Unknown = "Unknown";

        private Dictionary<int, string> _map = new Dictionary<int, string>()
        {
            { -1, All},
            { 0, "Slow" },
            { 1, "Casual" },
            { 2, "Normal" },
            { 3, "Fast" },
            { -2, Unknown }
        };

        public List<string> GetAll() => _map.Where(kv => kv.Key != -2).Select(kv => kv.Value).ToList();

        public string GetById(int id)
        {
            if (_map.ContainsKey(id)) return _map[id];
            return Unknown;
        }
    }
}
