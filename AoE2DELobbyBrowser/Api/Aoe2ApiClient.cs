using AoE2DELobbyBrowser.Models;
using DynamicData;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowser.Api
{
    internal class Aoe2ApiClient : IApiClient, IDisposable
    {
        //private const string getLobbiesUrl = "https://aoe2api.dryforest.net/api/v3/lobbies";
        private const string getLobbiesUrl = "https://localhost:7214/api/v3/lobbies";

        private readonly HttpClient _httpClient;
        private readonly SourceCache<Lobby, string> _items = new SourceCache<Lobby, string>(x => x.MatchId);

        public Aoe2ApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(20);
        }

        public IObservable<IChangeSet<Lobby, string>> Connect() => _items.Connect();

        public async Task Refresh(CancellationToken cancellationToken)
        {
            Log.Information("Refresh");
            var results = await GetAllLobbiesAsync(cancellationToken);
            if (results.Count == 0) return;

            var lobbies = results.Select(dto => Lobby.Create(dto));
            var keysToDelete = _items.Keys.ToHashSet();
            keysToDelete.ExceptWith(lobbies.Select(x => x.MatchId).ToList());
            var fvdLobbies = lobbies.Where(x => x.Name.ToLower().Contains("f") && x.GameType == "Scenario");
            foreach(var l in fvdLobbies)
            {
                Log.Information($"ApiClient f found {l.Name}");
            }
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
        public void Dispose()
        {
            _items.Clear();
            _items.Dispose();
        }
    }
}
