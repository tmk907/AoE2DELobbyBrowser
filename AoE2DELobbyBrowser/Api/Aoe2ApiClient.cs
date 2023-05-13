﻿using AoE2DELobbyBrowser.Models;
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
#if DEBUG
        private const string getLobbiesUrl = "https://aoe2api.dryforest.net/api/v3/lobbies";
        //private const string getLobbiesUrl = "https://localhost:7214/api/v3/lobbies";
#else
    private const string getLobbiesUrl = "https://aoe2api.dryforest.net/api/v3/lobbies";
#endif

        private readonly HttpClient _httpClient;
        private readonly SourceCache<Lobby, string> _itemsCache;
        private readonly IObservableCache<Lobby, string> _itemsObservableCache;

        public Aoe2ApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(20);
            _itemsCache = new SourceCache<Lobby, string>(x => x.MatchId);
            _itemsObservableCache = _itemsCache.AsObservableCache();
        }

        public IObservableCache<Lobby, string> Items => _itemsObservableCache;
        public IObservable<IChangeSet<Lobby, string>> Connect() => _itemsCache.Connect().RefCount();

        public async Task Refresh(CancellationToken cancellationToken)
        {
            Log.Debug("Refresh");
            var results = await GetAllLobbiesAsync(cancellationToken);
            if (results.Count == 0) return;

            var lobbies = results.Select(dto => Lobby.Create(dto));
            var keysToDelete = _itemsCache.Keys.ToHashSet();
            keysToDelete.ExceptWith(lobbies.Select(x => x.MatchId).ToList());
            _itemsCache.Edit(updater =>
            {
                updater.RemoveKeys(keysToDelete);
                updater.AddOrUpdate(lobbies);
            });
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

        public void Dispose()
        {
            _itemsCache.Clear();
            _itemsCache.Dispose();
        }
    }
}
