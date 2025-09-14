using AoE2DELobbyBrowser.WebApi.Database;
using AoE2DELobbyBrowser.WebApi.Reliclink;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AoE2DELobbyBrowser.WebApi
{
    public class DatabaseService
    {
        private readonly IDbContextFactory<MatchesDbContext> _dbFactory;
        private readonly ILogger<DatabaseService> _logger;

        public DatabaseService(IDbContextFactory<MatchesDbContext> dbFactory, ILogger<DatabaseService> logger)
        {
            _dbFactory = dbFactory;
            _logger = logger;
        }

        public async Task AddMatches(IEnumerable<Match> matches)
        {
            _logger.LogInformation("Add matches {count}", matches.Count());

            using var db = _dbFactory.CreateDbContext();

            db.Matches.AddRange(matches.Select(x => new MatchesTable
            {
                MatchId = x.Id,
                Json = JsonSerializer.Serialize(x)
            }));
            await db.SaveChangesAsync();
        }

        public async Task AddOrUpdateMatches(IEnumerable<Match> matches)
        {
            _logger.LogInformation("Update matches");

            using var db = _dbFactory.CreateDbContext();
            var ids = matches.Select(x => x.Id).ToList();
            var existingMatches = await db.Matches.Where(x => ids.Contains(x.MatchId)).ToListAsync();
            var toAdd = new List<Match>();

            foreach (var match in matches)
            {
                var existing = existingMatches.FirstOrDefault(x => x.MatchId == match.Id);
                if (existing != null)
                {
                    existing.Json = JsonSerializer.Serialize(match);
                    db.Matches.Update(existing);
                }
                else
                {
                    toAdd.Add(match);
                }
            }
            await db.SaveChangesAsync();
            await AddMatches(toAdd);
        }

        public async Task AddLobbies(IEnumerable<CachedLobby> lobbies)
        {
            _logger.LogInformation("Add lobbies {count}", lobbies.Count());

            using var db = _dbFactory.CreateDbContext();
            db.Lobbies.AddRange(lobbies.Select(x => new LobbiesTable
            {
                MatchId = x.MatchId,
                Name = x.Name,
                GameType = x.GameType,
                MapType = x.MapType,
                Speed = x.Speed,
                Dataset = x.Dataset,
                ModId = x.ModId,
                Scenario = x.Scenario,
                IsObservable = x.IsObservable,
                CreatedAt = x.CreatedAt,
                PlayerIds = x.PlayerIds,
                UpdatedAt = x.UpdatedAt
            }));
            await db.SaveChangesAsync();
        }

        public async Task AddOrUpdateLobbies(IEnumerable<CachedLobby> lobbies)
        {
            _logger.LogInformation("Update lobbies");

            using var db = _dbFactory.CreateDbContext();
            var ids = lobbies.Select(x => x.MatchId).ToList();
            var existingLobbies = await db.Lobbies.Where(x => ids.Contains(x.MatchId)).ToListAsync();
            var toAdd = new List<CachedLobby>();

            foreach (var lobby in lobbies)
            {
                var existing = existingLobbies.FirstOrDefault(x => x.MatchId == lobby.MatchId);
                if (existing != null)
                {
                    existing.Name = lobby.Name;
                    existing.GameType = lobby.GameType;
                    existing.MapType = lobby.MapType;
                    existing.Speed = lobby.Speed;
                    existing.Dataset = lobby.Dataset;
                    existing.ModId = lobby.ModId;
                    existing.Scenario = lobby.Scenario;
                    existing.IsObservable = lobby.IsObservable;
                    existing.UpdatedAt = lobby.UpdatedAt;
                    existing.PlayerIds = lobby.PlayerIds;

                    db.Lobbies.Update(existing);
                }
                else
                {
                    toAdd.Add(lobby);
                }
            }
            await db.SaveChangesAsync();
            await AddLobbies(toAdd);
        }

        public async Task<List<CachedLobby>> GetLobbies(CachedLobbiesQuery query)
        {
            using var db = _dbFactory.CreateDbContext();

            var results = db.Lobbies.AsQueryable();

            if (query.Name != null)
            {
                results = results.Where(x => EF.Functions.Like(x.Name, $"%{query.Name}%"));
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
                results = results.Where(x => EF.Functions.Like(x.Dataset, $"%{query.Dataset}%"));
            }
            if (query.ModId != null)
            {
                results = results.Where(x => EF.Functions.Like(x.ModId, $"{query.ModId}%"));
            }
            if (query.Scenario != null)
            {
                results = results.Where(x => EF.Functions.Like(x.Scenario, $"%{query.Scenario}%"));
            }
            if (query.PlayerId != null)
            {
                results = results.Where(x => x.PlayerIds.Contains(query.PlayerId.Value));
            }
            if (query.IsObservable != null)
            {
                results = results.Where(x => x.IsObservable == query.IsObservable.Value);
            }
            if (query.CreatedAt != null)
            {
                results = results.Where(x => x.CreatedAt.Date == query.CreatedAt.Value.Date);
            }
            var list = await results.OrderBy(x => x.CreatedAt).ToListAsync();
            return list.Select(x => new CachedLobby(x.MatchId, x.PlayerIds)
            {
                Name = x.Name,
                GameType = x.GameType,
                MapType = x.MapType,
                Speed = x.Speed,
                Dataset = x.Dataset,
                ModId = x.ModId,
                Scenario = x.Scenario,
                IsObservable = x.IsObservable,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                MatchId = x.MatchId,
                PlayerIds = x.PlayerIds
            }).ToList();
        }
    }
}
