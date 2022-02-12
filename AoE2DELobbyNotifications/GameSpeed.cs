﻿using System.Collections.Generic;
using System.Linq;

namespace AoE2DELobbyNotifications
{
    public class GameSpeed
    {
        public const string All = "All";

        private Dictionary<int, string> _map = new Dictionary<int, string>()
        {
            { -1, All},
            { 0, "Slow" },
            { 1, "Casual" },
            { 2, "Normal" },
            { 3, "Fast" },
        };

        public List<string> GetAll() => _map.Select(kv => kv.Value).ToList();

        public string GetById(int id)
        {
            if (_map.ContainsKey(id)) return _map[id];
            return "Unknown";
        }
    }
}
