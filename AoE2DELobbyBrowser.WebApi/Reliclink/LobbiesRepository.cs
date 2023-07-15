using AoE2DELobbyBrowser.WebApi.Dto;

namespace AoE2DELobbyBrowser.WebApi.Reliclink
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

        public async Task<IEnumerable<LobbyDto>> GetLobbiesAsync()
        {
            var advertisement = await _apiCache.GetOrCreateAsync("advertisement", GetAllAdvertisementsAsync);
            var lobbies = GetLobbies(advertisement);
            return lobbies;
        }

        private async Task<Advertisement> GetAllAdvertisementsAsync()
        {
            var advertisement = new Advertisement() { Matches = new List<Match>(), Avatars = new List<Avatar>() };

            var start = 0;
            var adv = await GetAdvertisementAsync(start);
            while ((adv?.Matches?.Count ?? 0) > 0)
            {
                if (adv?.Matches != null) advertisement.Matches.AddRange(adv.Matches);
                if (adv?.Avatars != null) advertisement.Avatars.AddRange(adv.Avatars);
                start += 100;
                adv = await GetAdvertisementAsync(start);
            }

            advertisement.Matches = advertisement.Matches.DistinctBy(x=>x.Id).ToList();
            advertisement.Avatars = advertisement.Avatars.DistinctBy(x=>x.ProfileId).ToList();

            return advertisement;
        }

        private async Task<Advertisement?> GetAdvertisementAsync(int start)
        {
            var url = $"https://aoe-api.reliclink.com/community/advertisement/findAdvertisements?title=age2&start={start}";
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                _logger.LogInformation("Get lobbies from reliclink");

                var advertisement = await httpClient.GetFromJsonAsync<Advertisement>(url);

                _logger.LogInformation($"Total lobbies {advertisement?.Matches?.Count ?? 0}");

                return advertisement;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get lobbies from aoe2insights");
                return null;
            }
        }

        private List<LobbyDto> GetLobbies(Advertisement? advertisements)
        {
            var result = new List<LobbyDto>();

            if (advertisements == null) return result;

            foreach (var match in advertisements.Matches)
            {
                try
                {
                    var options = OptionsDecoder.DecodedToDict(OptionsDecoder.DecodeOptions(match.Options));
                    result.Add(Create(match, options, advertisements.Avatars));
                }
                catch (Exception) { }
            }

            return result.Where(x => x.Players.Count < x.NumSlots).ToList();
        }
 
        private LobbyDto Create(Match match, Dictionary<int, string> options, List<Avatar> avatars)
        {
            var gameId = int.Parse(options.GetValueOrDefault(OptionsDecoder.GameTypeKey, "-1"));
            var mapId = int.Parse(options.GetValueOrDefault(OptionsDecoder.MapTypeKey, "-1"));
            var speedId = int.Parse(options.GetValueOrDefault(OptionsDecoder.GameSpeedKey, "-1"));
            return new LobbyDto
            {
                GameType = GameTypeConverter.ToName(gameId),
                SteamLobbyId = match.Steamlobbyid?.ToString() ?? "",
                MatchId = match.Id.ToString(),
                MapType = MapTypeConverter.ToName(mapId),
                Name = match.Description,
                NumSlots = match.Maxplayers,
                Players = match.Matchmembers
                    .OrderBy(x => (x.ProfileId == match.HostProfileId) ? 0 : 1)
                    .Select((x, i) => Create(i, x, avatars))
                    .ToList(),
                Speed = GameSpeedConverter.ToName(speedId)
            };
        }

        private PlayerDto Create(int i, Matchmember dto, List<Avatar> avatars)
        {
            var avatar = avatars.FirstOrDefault(x => x.ProfileId == dto.ProfileId);
            if (avatar == null)
            {
                return new PlayerDto
                {
                    Country = "??",
                    Name = "",
                    Slot = i,
                    ProfileId = dto.ProfileId.ToString(),
                    SteamProfileId = ""
                };
            }
            else
            {
                return new PlayerDto
                {
                    Country = avatar.Country.ToUpper(),
                    Name = avatar.Alias,
                    Slot = i,
                    ProfileId = dto.ProfileId.ToString(),
                    SteamProfileId = avatar.Name.Replace("/steam/", "")
                };
            }
        }
    }
}
