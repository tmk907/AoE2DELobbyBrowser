using AoE2DELobbyBrowser.WebApi.Reliclink;
using System.Collections.Concurrent;

namespace AoE2DELobbyBrowser.WebApi
{
    public class CachedLobbyService
    {
        private readonly ConcurrentDictionary<int, CachedLobby> _lobbies;
        private readonly ILogger<CachedLobbyService> _logger;
        private readonly TimeSpan _maxDuration = TimeSpan.FromHours(12);

        public CachedLobbyService(ILogger<CachedLobbyService> logger)
        {
            _lobbies = new ConcurrentDictionary<int, CachedLobby>();
            _logger = logger;
        }

        public void UpdateLobbies(Advertisement advertisement)
        {
            var toDelete = _lobbies.Values
                .Where(x => (DateTime.UtcNow - x.UpdatedAt) > _maxDuration)
                .Select(x => x.MatchId).ToList();

            foreach (var id in toDelete)
            {
                _logger.LogInformation("Remove lobby {matchId} {updatedAt}", id, _lobbies[id].UpdatedAt);
                _lobbies.TryRemove(id, out _);
            }

            foreach (var match in advertisement.Matches)
            {
                var options = OptionsDecoder.DecodedToDict(OptionsDecoder.DecodeOptions(match.Options));
                
                if (_lobbies.TryGetValue(match.Id, out var lobby))
                {
                    lobby.UpdatePlayers(match.Matchmembers.Select(x => x.ProfileId));
                }
                else
                {
                    var newLobby = new CachedLobby(match.Id, match.Matchmembers.Select(x => x.ProfileId))
                    {
                        Name = match.Description,
                        GameType = options.GetValueOrDefault(OptionsDecoder.GameTypeKey, ""),
                        MapType = options.GetValueOrDefault(OptionsDecoder.MapTypeKey, ""),
                        Speed = options.GetValueOrDefault(OptionsDecoder.GameSpeedKey, ""),
                        Dataset = options.GetValueOrDefault(OptionsDecoder.Dataset, ""),
                        ModId = options.GetValueOrDefault(OptionsDecoder.ModId, ""),
                        Scenario = options.GetValueOrDefault(OptionsDecoder.Scenario, ""),
                        IsObservable = match.IsObservable
                    };
                    _lobbies.TryAdd(match.Id, newLobby);
                    _logger.LogInformation("New lobby {matchId}", match.Id);
                }
            }
        }

        public List<CachedLobby> GetMatches(CachedLobbiesQuery query)
        {
            var results = _lobbies.Values.AsEnumerable();
            if (query.Name != null)
            {
                results = results.Where(x => x.Name.Contains(query.Name, StringComparison.InvariantCultureIgnoreCase));
            }
            if (query.GameType != null)
            {
                results = results.Where(x => x.GameType == query.GameType);
            }
            if (query.MapType != null)
            {
                results = results.Where(x => x.MapType == query.MapType);
            }
            if (query.Speed != null)
            {
                results = results.Where(x => x.Speed == query.Speed);
            }
            if (query.Dataset != null)
            {
                results = results.Where(x => x.Dataset.Contains(query.Dataset, StringComparison.InvariantCultureIgnoreCase));
            }
            if (query.ModId != null)
            {
                results = results.Where(x => x.ModId.StartsWith(query.ModId));
            }
            if (query.Scenario != null)
            {
                results = results.Where(x => x.Scenario.Contains(query.Scenario, StringComparison.InvariantCultureIgnoreCase));
            }
            if (query.PlayerId != null)
            {
                results = results.Where(x => x.PlayerIds.Contains(query.PlayerId.Value));
            }
            if (query.IsObservable != null)
            {
                results = results.Where(x => x.IsObservable == query.IsObservable.Value);
            }

            return results.OrderBy(x => x.CreatedAt).ToList();
        }
    }
}
