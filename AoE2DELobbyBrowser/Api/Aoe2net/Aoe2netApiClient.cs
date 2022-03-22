using DynamicData;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowser.Api.Aoe2net
{
    internal class Aoe2netApiClient : IApiClient
    {
        private const string getLobbiesUrl = "https://aoe2.net/api/lobbies?game=aoe2de";

        private readonly HttpClient _httpClient;
        private readonly SourceCache<Lobby, string> _items = new SourceCache<Lobby, string>(x => x.LobbyId);

        public Aoe2netApiClient()
        {
            _httpClient = new HttpClient();
        }

        public IObservable<IChangeSet<Lobby, string>> Connect() => _items.Connect();

        public async Task Refresh(CancellationToken cancellationToken)
        {
            Log.Information("Refresh");
            var results = await GetAllLobbiesAsync(cancellationToken);
            var lobbies = results.Select(dto => Lobby.Create(dto));
            var keysToDelete = _items.Keys.ToHashSet();
            keysToDelete.ExceptWith(lobbies.Select(x => x.LobbyId).ToList());
            _items.Edit(updater =>
            {
                updater.RemoveKeys(keysToDelete);
                updater.AddOrUpdate(lobbies);
            });
        }

        public async Task<List<LobbyDto>> GetAllLobbiesAsync(CancellationToken cancellationToken)
        {
            Log.Information("GetAllLobbiesAsync");
            try
            {
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
