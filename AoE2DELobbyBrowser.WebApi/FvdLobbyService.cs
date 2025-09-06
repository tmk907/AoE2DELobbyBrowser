using AoE2DELobbyBrowser.WebApi.Reliclink;
using System.Collections.Concurrent;

namespace AoE2DELobbyBrowser.WebApi
{
    public class FvdLobbyService
    {
        private readonly ConcurrentDictionary<int, FvdMatch> _fvdLobbies;
        private readonly ILogger<FvdLobbyService> _logger;
        private readonly TimeSpan _maxDuration = TimeSpan.FromHours(24);

        public FvdLobbyService(ILogger<FvdLobbyService> logger)
        {
            _fvdLobbies = new ConcurrentDictionary<int, FvdMatch>();
            _logger = logger;
        }

        public void UpdateFvdMatches(Advertisement advertisement)
        {
            var toDelete = _fvdLobbies.Values
                .Where(x => (DateTime.UtcNow - x.UpdatedAt) > _maxDuration)
                .Select(x => x.MatchId).ToList();
            foreach (var id in toDelete)
            {
                _logger.LogInformation("Remove fvd match {matchId} {updatedAt}", id, _fvdLobbies[id].UpdatedAt);
                _fvdLobbies.TryRemove(id, out _);
            }

            foreach (var match in advertisement.Matches)
            {
                var options = OptionsDecoder.DecodedToDict(OptionsDecoder.DecodeOptions(match.Options));
                var modId = options.GetValueOrDefault(OptionsDecoder.ModId, "");
                if (modId == "23222+" || modId == "23222")
                {
                    if (_fvdLobbies.TryGetValue(match.Id, out var fvdMatch))
                    {
                        fvdMatch.UpdatePlayers(match.Matchmembers.Select(x => x.ProfileId));
                    }
                    else
                    {
                        _fvdLobbies.TryAdd(match.Id, new FvdMatch(match.Id, match.Matchmembers.Select(x => x.ProfileId)));
                        _logger.LogInformation("New fvd match {matchId}", match.Id);
                    }
                }
            }
        }

        public List<FvdMatch> GetFvdMatches()
        {
            return _fvdLobbies.Values.ToList();
        }
    }
}
