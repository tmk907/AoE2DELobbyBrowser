using AoE2DELobbyBrowser.WebApi.Dto;

namespace AoE2DELobbyBrowser.WebApi.Reliclink
{
    public class LobbiesRepository
    {
        private readonly ILogger<LobbiesRepository> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiCache _apiCache;
        private readonly CachedLobbyService _fvdLobbyService;

        public LobbiesRepository(ILogger<LobbiesRepository> logger, IHttpClientFactory httpClientFactory, 
            ApiCache apiCache, CachedLobbyService fvdLobbyService)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _apiCache = apiCache;
            _fvdLobbyService = fvdLobbyService;
        }

        public async Task RefreshCacheAsync()
        {
            _logger.LogInformation("Refresh cache");

            var advertisements = await GetAllAdvertisementsAsync();
            var lobbies = GetLobbies(advertisements);
            try
            {
                await _fvdLobbyService.UpdateLobbies(advertisements);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateLobbies");
            }

            _apiCache.Set(ApiCache.LobbiesKey, lobbies);
        }

        private async Task<Advertisement> GetAllAdvertisementsAsync()
        {
            var advertisement = new Advertisement() { Matches = new List<Match>(), Avatars = new List<Avatar>() };

            var results = await Task.WhenAll(
                GetAdvertisementAsync(0), 
                GetAdvertisementAsync(100), 
                GetAdvertisementAsync(200));

            foreach(var adv in results)
            {
                if (adv?.Matches != null) advertisement.Matches.AddRange(adv.Matches);
                if (adv?.Avatars != null) advertisement.Avatars.AddRange(adv.Avatars);
            }

            if ((results.LastOrDefault()?.Matches?.Count ?? 0) > 0)
            {
                var start = 300;
                var adv = await GetAdvertisementAsync(start);
                while ((adv?.Matches?.Count ?? 0) > 0)
                {
                    if (adv?.Matches != null) advertisement.Matches.AddRange(adv.Matches);
                    if (adv?.Avatars != null) advertisement.Avatars.AddRange(adv.Avatars);
                    start += 100;
                    adv = await GetAdvertisementAsync(start);
                }
            }

            advertisement.Matches = advertisement.Matches.DistinctBy(x=>x.Id).ToList();
            advertisement.Avatars = advertisement.Avatars.DistinctBy(x=>x.ProfileId).ToList();
            _logger.LogInformation("Found {matches} matches, {players} players",
                advertisement.Matches.Count, advertisement.Avatars.Count);

            return advertisement;
        }

        private async Task<Advertisement?> GetAdvertisementAsync(int start)
        {
            var url = $"https://aoe-api.worldsedgelink.com/community/advertisement/findAdvertisements?title=age2&start={start}";
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var advertisement = await httpClient.GetFromJsonAsync<Advertisement>(url);
                return advertisement;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAdvertisementAsync");
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
                    result.Add(ToLobby(match, options, advertisements.Avatars));
                }
                catch (Exception) { }
            }

            return result.Where(x => x.Players.Count < x.NumSlots).ToList();
        }
 
        private LobbyDto ToLobby(Match match, Dictionary<int, string> options, List<Avatar> avatars)
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
                    .Select((x, i) => ToPlayer(i, x, avatars))
                    .ToList(),
                Speed = GameSpeedConverter.ToName(speedId)
            };
        }

        private PlayerDto ToPlayer(int i, Matchmember dto, List<Avatar> avatars)
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
