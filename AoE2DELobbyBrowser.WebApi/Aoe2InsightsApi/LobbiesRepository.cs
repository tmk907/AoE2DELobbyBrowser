using System.Text.Json;
using System.Text.Json.Serialization;

namespace AoE2DELobbyBrowser.WebApi.Aoe2InsightsApi
{
    public class LobbiesRepository
    {
        private readonly ILogger<LobbiesRepository> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiCache _apiCache;

        public LobbiesRepository(ILogger<LobbiesRepository> logger, IHttpClientFactory httpClientFactory, ApiCache apiCache)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _apiCache = apiCache;
        }

        public async Task<IEnumerable<Dto.LobbyDto>> GetLobbiesAsync()
        {
            var lobbies = await _apiCache.GetOrCreateAsync("lobbies", GetLobbiesFromApiAsync);
            return lobbies;
        }

        private async Task<List<Dto.LobbyDto>> GetLobbiesFromApiAsync()
        {
            var url = "https://www.aoe2insights.com/lobbies/api";
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                _logger.LogInformation("Get lobbies from aoe2insights");

                var response = await httpClient.GetStringAsync(url);
                var result = JsonSerializer.Deserialize<List<LobbyDto>>(response);
                result = result ?? new List<LobbyDto>();

                _logger.LogInformation($"Total lobbies {result.Count}");

                return result.Select(x => Create(x)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get lobbies from aoe2insights");
                return new List<Dto.LobbyDto>();
            }
        }

        private Dto.LobbyDto Create(LobbyDto dto)
        {
            return new Dto.LobbyDto
            {
                GameType = GameTypeConverter.ToName(dto.GameType),
                LobbyId = "",
                MatchId = dto.Id.ToString(),
                MapType = MapTypeConverter.ToName(dto.MapType),
                Name = dto.Name,
                NumPlayers = dto.Players.Count,
                NumSlots = dto.NumSlots,
                Opened = 0,
                Players = dto.Players.Select(x => Create(x)).ToList(),
                Speed = GameSpeedConverter.ToName(dto.Speed)
            };
        }

        private Dto.PlayerDto Create(PlayerDto dto)
        {
            return new Dto.PlayerDto
            {
                Country = "",
                Drops = 0,
                Games = 0,
                Name = dto.Name,
                Rating = 0,
                Slot = dto.Slot,
                Streak = 0,
                Wins = 0
            };
        }
    }
}
