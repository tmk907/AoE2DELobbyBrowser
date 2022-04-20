using System.Collections.Concurrent;

namespace AoE2DELobbyBrowser.WebApi
{
    public class LobbiesRepository
    {
        private readonly ILogger<LobbiesRepository> _logger;
        private readonly ConcurrentDictionary<string, LobbyDto> _lobbies;

        private static SemaphoreSlim semaphore;

        public LobbiesRepository(ILogger<LobbiesRepository> logger)
        {
            _logger = logger;
            _lobbies = new ConcurrentDictionary<string, LobbyDto>();
            semaphore = new SemaphoreSlim(0, 1);
        }

        public void AddLobbies(IEnumerable<Aoe2netWebsocket.LobbyDto> newLobbies)
        {
            semaphore.Wait();
            try
            {
                // remove lobbies where first player and game name is the same
                var nameToLobbyId = _lobbies.ToDictionary(x => GetHostAndGameName(x.Value), x => x.Key);
                foreach(var lobby in newLobbies)
                {
                    var name = GetHostAndGameName(Create(lobby));
                    if (nameToLobbyId.TryGetValue(name, out var lobbyId))
                    {
                        _lobbies.Remove(lobbyId, out _);
                    }
                }

                _logger.LogDebug($"Add lobbies {newLobbies.Count()}");
                foreach (var lobby in newLobbies.Select(x => Create(x)))
                {
                    _lobbies.AddOrUpdate(lobby.LobbyId, lobby, (key, oldLobby) => lobby);
                }

                var notActive = newLobbies.Where(x => !x.Active).Select(x => x.SteamLobbyId).ToList();
                if (notActive.Count > 0)
                {
                    _logger.LogDebug($"Not active lobbies {notActive.Count}");
                }
                foreach (var key in notActive)
                {
                    _lobbies.Remove(key, out LobbyDto _);
                }

                var full = _lobbies.Where(x => x.Value.NumSlots == x.Value.NumPlayers).Select(x => x.Key).ToList();
                _logger.LogDebug($"Full lobbies {full.Count} ");
                foreach (var key in full)
                {
                    _lobbies.Remove(key, out LobbyDto _);
                }

                var numSlotsZero = _lobbies.Where(x => x.Value.NumSlots == 0).Select(x => x.Key).ToList();
                if (numSlotsZero.Count > 0)
                {
                    _logger.LogDebug($"numSlotsZero lobbies {numSlotsZero.Count}");
                }
                foreach (var key in numSlotsZero)
                {
                    _lobbies.Remove(key, out LobbyDto _);
                }

                _logger.LogInformation($"Total lobbies {_lobbies.Count}");
            }
            finally
            {
                semaphore.Release();
            }
        }

        public IEnumerable<LobbyDto> GetLobbies()
        {
            _logger.LogInformation("GetLobbies");
            return _lobbies.Select(x => x.Value).ToList();
        }

        public void Clear()
        {
            _logger.LogInformation("ClearLobbies");
            _lobbies.Clear();
        }

        private LobbyDto Create(Aoe2netWebsocket.LobbyDto dto)
        {
            return new LobbyDto
            {
                GameType = dto.GameType,
                LobbyId = dto.SteamLobbyId,
                MapType = dto.Location,
                Name = dto.Name,
                NumPlayers = dto.NumPlayers,
                NumSlots = dto.NumSlots,
                Opened = dto.Opened,
                Players = dto.Players.Select(x => Create(x)).ToList(),
                Speed = dto.Speed
            };
        }

        private string GetHostAndGameName(LobbyDto lobby)
        {
            return $"{lobby.Players.FirstOrDefault()?.Name ?? ""}~{lobby.Name}";
        }

        private PlayerDto Create(Aoe2netWebsocket.PlayerDto dto)
        {
            return new PlayerDto
            {
                //Avatar = dto.Avatar,
                Country = dto.CountryCode,
                Drops = dto.Drops,
                Games = dto.Games,
                Name = dto.Name,
                Rating = dto.Rating,
                Slot = dto.Slot,
                Streak = dto.Streak,
                Wins = dto.Wins
            };
        }
    }
}
