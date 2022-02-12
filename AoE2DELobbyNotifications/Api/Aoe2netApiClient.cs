using DynamicData;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AoE2DELobbyNotifications.Api
{
    internal class Aoe2netApiClient
    {
        private const string getLobbiesUrl = "https://aoe2.net/api/lobbies?game=aoe2de";

        private readonly HttpClient _httpClient;
        private readonly SourceCache<LobbyDto, string> _items = new SourceCache<LobbyDto, string>(x => x.LobbyId);

        public Aoe2netApiClient()
        {
            _httpClient = new HttpClient();
        }

        public IObservable<IChangeSet<LobbyDto,string>> Connect() => _items.Connect();

        public async Task Refresh(CancellationToken cancellationToken)
        {
            var results = await GetAllLobbiesAsync(cancellationToken);
            var keysToDelete = _items.Keys.ToHashSet();
            keysToDelete.ExceptWith(results.Select(x => x.LobbyId).ToList());
            _items.RemoveKeys(keysToDelete);
            //var newKeys = results.Select(x => x.LobbyId).ToHashSet();
            //newKeys.ExceptWith(_items.Keys);
            
            _items.AddOrUpdate(results);
        }

        public async Task<List<LobbyDto>> GetAllLobbiesAsync(CancellationToken cancellationToken)
        {
            Log.Information("GetAllLobbiesAsync");
            try
            {
                //var result = await getLobbiesUrl.GetJsonAsync<List<LobbyDto>>(cancellationToken);
                var result = await _httpClient.GetFromJsonAsync<List<LobbyDto>>(getLobbiesUrl, cancellationToken);
                Log.Information($"Found {result.Count} lobbies");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return new List<LobbyDto>();
            }
        }
    }
}
