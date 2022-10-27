using AoE2DELobbyBrowser.WebApi.v2.Dto;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace AoE2DELobbyBrowser.WebApi.v2.Reliclink
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
            var advertisement = await _apiCache.GetOrCreateAsync("advertisement", GetAdvertisementAsync);
            var lobbies = GetLobbies(advertisement);
            return lobbies;
        }

        private async Task<Advertisement?> GetAdvertisementAsync()
        {
            var url = "https://aoe-api.reliclink.com/community/advertisement/findAdvertisements?title=age2";
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
                    var options = DecodedToDict(DecodeOptions(match.Options));
                    result.Add(Create(match, options, advertisements.Avatars));
                }
                catch (Exception) { }
            }

            return result.Where(x => x.Players.Count < x.NumSlots).ToList();
        }

        private const int GameTypeKey = 6;
        private const int MapTypeKey = 11;
        private const int GameSpeedKey = 42;

        private LobbyDto Create(Match match, Dictionary<int, string> options, List<Avatar> avatars)
        {
            return new LobbyDto
            {
                GameType = GameTypeConverter.ToName(int.Parse(options[GameTypeKey])),
                LobbyId = match.Steamlobbyid?.ToString(),
                MatchId = match.Id.ToString(),
                MapType = MapTypeConverter.ToName(int.Parse(options[MapTypeKey])),
                Name = match.Description,
                NumPlayers = match.Matchmembers.Count,
                NumSlots = match.Maxplayers,
                Opened = 0,
                Players = match.Matchmembers.Select((x, i) => Create(i, x, avatars)).ToList(),
                Speed = GameSpeedConverter.ToName(int.Parse(options[GameSpeedKey]))
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
                    Drops = 0,
                    Games = 0,
                    Name = "",
                    Rating = 0,
                    Slot = i,
                    Streak = 0,
                    Wins = 0
                };
            }
            else
            {
                return new PlayerDto
                {
                    Country = avatar.Country.ToUpper(),
                    Drops = 0,
                    Games = 0,
                    Name = avatar.Alias,
                    Rating = 0,
                    Slot = i,
                    Streak = 0,
                    Wins = 0
                };
            }
        }

        private byte[] DecodeZLib(byte[] input)
        {
            var inputStream = new MemoryStream(input);
            using (var zipInput = new InflaterInputStream(inputStream))
            {
                using (var resultStream = new MemoryStream())
                {
                    zipInput.CopyTo(resultStream);
                    return resultStream.ToArray();
                }
            }
        }

        private string DecodeOptions(string input)
        {
            byte[] decoded = System.Convert.FromBase64String(input);
            byte[] unzipped = DecodeZLib(decoded);
            var text = System.Text.Encoding.ASCII.GetString(unzipped).Replace("\"", "");
            return System.Text.Encoding.ASCII.GetString(System.Convert.FromBase64String(text));
        }

        private Dictionary<int, string> DecodedToDict(string decoded)
        {
            var separators = new[] { "\u0003\0\0\0", "\u0004\0\0\0", "\u0005\0\0\0", "\u0006\0\0\0" };
            var splitted = decoded.Split(separators, StringSplitOptions.None);
            return splitted.Where(x => x.Contains(':'))
                .ToDictionary(x => int.Parse(x.Split(':')[0]), x => x.Split(':')[1]);
        }
    }
}
