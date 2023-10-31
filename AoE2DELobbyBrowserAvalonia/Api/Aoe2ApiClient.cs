using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowser.Api
{
    internal class Aoe2ApiClient : IApiClient
    {
#if DEBUG
        //public const string BaseUrl = "localhost:7214";
        public const string BaseUrl = "aoe2api.dryforest.net";
#else
    public const string BaseUrl = "aoe2api.dryforest.net";
#endif

        private const string getLobbiesUrl = $"https://{BaseUrl}/api/v3/lobbies";
        private readonly HttpClient _httpClient;

        public Aoe2ApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(20);
        }

        public async Task<List<LobbyDto>> GetAllLobbiesAsync(CancellationToken cancellationToken)
        {
            Log.Debug("GetAllLobbiesAsync");
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<LobbyDto>>(getLobbiesUrl, cancellationToken);
                Log.Debug($"Found {result.Count} lobbies");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return new List<LobbyDto>();
            }
        }

        public async Task<List<SteamPlayerDto>> GetSteamPlayersAsync(string ids)
        {
            var url = $"https://{BaseUrl}/api/v3/players?ids={ids}";
            return await _httpClient.GetFromJsonAsync<List<SteamPlayerDto>>(url);
        }
    }
}
