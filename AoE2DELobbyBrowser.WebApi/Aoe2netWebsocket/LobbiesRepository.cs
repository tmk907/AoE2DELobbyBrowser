using System.Collections.Concurrent;

namespace AoE2DELobbyBrowser.WebApi.Aoe2netWebsocket
{
    public class LobbiesRepository
    {
        private readonly ILogger<LobbiesRepository> _logger;
        private readonly ConcurrentDictionary<string, v2.Dto.LobbyDto> _lobbies;

        private static SemaphoreSlim _semaphore;

        public LobbiesRepository(ILogger<LobbiesRepository> logger)
        {
            _logger = logger;
            _lobbies = new ConcurrentDictionary<string, v2.Dto.LobbyDto>();
            _semaphore = new SemaphoreSlim(1, 1);
        }

        public void AddLobbies(IEnumerable<LobbyDto> newLobbies)
        {
            _semaphore.Wait();
            try
            {
                // remove lobbies where first player and game name is the same
                var nameToLobbyId = new Dictionary<string, string>();
                foreach (var lobby in _lobbies)
                {
                    var key = GetHostAndGameName(lobby.Value);
                    nameToLobbyId.TryAdd(key, lobby.Key);
                }
                foreach (var lobby in newLobbies)
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
                    _lobbies.Remove(key, out v2.Dto.LobbyDto _);
                }

                var full = _lobbies.Where(x => x.Value.NumSlots == x.Value.NumPlayers).Select(x => x.Key).ToList();
                _logger.LogDebug($"Full lobbies {full.Count} ");
                foreach (var key in full)
                {
                    _lobbies.Remove(key, out v2.Dto.LobbyDto _);
                }

                var numSlotsZero = _lobbies.Where(x => x.Value.NumSlots == 0).Select(x => x.Key).ToList();
                if (numSlotsZero.Count > 0)
                {
                    _logger.LogDebug($"numSlotsZero lobbies {numSlotsZero.Count}");
                }
                foreach (var key in numSlotsZero)
                {
                    _lobbies.Remove(key, out v2.Dto.LobbyDto _);
                }

                _logger.LogInformation($"Total lobbies {_lobbies.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public IEnumerable<v2.Dto.LobbyDto> GetLobbies()
        {
            _logger.LogInformation("GetLobbies");
            return _lobbies.Select(x => x.Value).ToList();
        }

        public void Clear()
        {
            _logger.LogInformation("ClearLobbies");
            _lobbies.Clear();
        }

        private v2.Dto.LobbyDto Create(LobbyDto dto)
        {
            return new v2.Dto.LobbyDto
            {
                GameType = dto.GameType,
                LobbyId = dto.SteamLobbyId,
                MatchId = dto.Id,
                MapType = dto.Location,
                Name = dto.Name,
                NumPlayers = dto.NumPlayers,
                NumSlots = dto.NumSlots,
                Opened = dto.Opened,
                Players = dto.Players.Select(x => Create(x)).ToList(),
                Speed = dto.Speed
            };
        }

        private string GetHostAndGameName(v2.Dto.LobbyDto lobby)
        {
            return $"{lobby.Players.FirstOrDefault()?.Name ?? ""}~{lobby.Name}~{lobby.MapType}";
        }

        private v2.Dto.PlayerDto Create(PlayerDto dto)
        {
            return new v2.Dto.PlayerDto
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
