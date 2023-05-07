using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Helpers;
using DynamicData;
using Serilog;


namespace AoE2DELobbyBrowser.Services
{
    public interface IPlayersService
    {
        IObservableCache<SteamPlayerDto, string> Items { get; }
        Task AddFriendAsync(string id);
        bool IsValidId(string id);
        Task ReloadAsync();
        Task RemoveFriendAsync(string id);
        Task UpdatePlayersFromSteamAsync();
    }

    public class PlayersService : IPlayersService
    {
        private HttpClient _httpClient;
        private ApplicationDataStorageHelper _storageHelper;

        private readonly SourceCache<SteamPlayerDto, string> _itemsSource;

        public PlayersService()
        {
            _httpClient = new HttpClient();
            _storageHelper = ApplicationDataStorageHelper.GetCurrent(new AppDataJsonSerializer());
            _itemsSource = new SourceCache<SteamPlayerDto, string>(x => x.SteamId);
            Items = _itemsSource.AsObservableCache();
        }

        public IObservableCache<SteamPlayerDto, string> Items { get; }

        public bool IsValidId(string id)
        {
            if (id != null &&
                id.Length == 17 && 
                long.TryParse(id, out var _)) return true;
            return false;
        }

        public async Task ReloadAsync()
        {
            var friends = await GetFriendsListAsync();
            _itemsSource.Clear();
            _itemsSource.AddOrUpdate(friends);
        }

        public async Task AddFriendAsync(string id)
        {
            if (!IsValidId(id)) return;

            var savedPlayers = await GetFriendsListAsync();
            var player = savedPlayers.FirstOrDefault(x => x.SteamId == id);
            if (player == null)
            {
                var url = $"https://aoe2api.dryforest.net/api/v3/players?ids={id}";
                var players = await _httpClient.GetFromJsonAsync<List<SteamPlayerDto>>(url);
                player = players.FirstOrDefault();
                if (player != null)
                {
                    savedPlayers.Add(player);
                    await SaveFriendsListAsync(savedPlayers);
                    _itemsSource.AddOrUpdate(player);
                }
            }
        }

        public async Task RemoveFriendAsync(string id)
        {
            var savedPlayers = await GetFriendsListAsync();
            var toRemove = savedPlayers.FirstOrDefault(x => x.SteamId == id);
            if (toRemove != null)
            {
                savedPlayers.Remove(toRemove);
                await SaveFriendsListAsync(savedPlayers);
                _itemsSource.Remove(toRemove);
            }
        }

        public async Task UpdatePlayersFromSteamAsync()
        {
            try
            {
                var players = await GetFriendsListAsync();
                if (players.Count == 0) return;

                var ids = string.Join(',', players.Select(x => x.SteamId));
                var url = $"https://aoe2api.dryforest.net/api/v3/players?ids={ids}";
                players = await _httpClient.GetFromJsonAsync<List<SteamPlayerDto>>(url);
                
                await SaveFriendsListAsync(players);
                _itemsSource.Edit(u =>
                {
                    u.Clear();
                    u.AddOrUpdate(players);
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        private async Task SaveFriendsListAsync(IEnumerable<SteamPlayerDto> players)
        {
            await _storageHelper.CreateFileAsync("favoritePlayers.json", players);
        }

        private async Task<List<SteamPlayerDto>> GetFriendsListAsync()
        {
            try
            {
                return await _storageHelper.ReadFileAsync("favoritePlayers.json", new List<SteamPlayerDto>());
            }
            catch (FileNotFoundException)
            {
                await _storageHelper.CreateFileAsync("favoritePlayers.json", new List<SteamPlayerDto>());
                return new List<SteamPlayerDto>();
            }
        }
    }
}
