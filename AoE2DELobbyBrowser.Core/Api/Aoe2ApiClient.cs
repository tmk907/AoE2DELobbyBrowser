using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowser.Core.Api
{
    public class Aoe2ApiClient : IApiClient
    {
        private readonly string _baseUrl;
        private readonly string getLobbiesUrl;
        private readonly HttpClient _httpClient;

        public Aoe2ApiClient(IConfiguration config)
        {
            _baseUrl = config.ApiBaseUrl;
            getLobbiesUrl = $"https://{_baseUrl}/api/v3/lobbies";
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(20);
        }

        public async Task<List<LobbyDto>> GetAllLobbiesAsync(CancellationToken cancellationToken)
        {
            Log.Debug("GetAllLobbiesAsync");
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<LobbyDto>>(getLobbiesUrl, cancellationToken);
                Log.Debug($"Found {result?.Count ?? 0} lobbies");
                return result ?? new List<LobbyDto>();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return new List<LobbyDto>();
            }
        }

        public async Task<List<SteamPlayerDto>> GetSteamPlayersAsync(string ids)
        {
            var url = $"https://{_baseUrl}/api/v3/players?ids={ids}";
            return await _httpClient.GetFromJsonAsync<List<SteamPlayerDto>>(url);
        }
    }
}
