using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AoE2DELobbyBrowser.Api;
using CommunityToolkit.WinUI.Helpers;
using DynamicData;
using Serilog;


namespace AoE2DELobbyBrowser.Services
{
    public interface IPlayersService
    {
        IObservableCache<SteamPlayerDto, string> AllPlayers { get; }

        Task AddFriendAsync(string id);
        Task<List<SteamPlayerDto>> GetFriendsListAsync();
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

            AllPlayers = _itemsSource.AsObservableCache();

            Observable
                .FromAsync(GetFriendsListAsync)
                .Do(list => _itemsSource.AddOrUpdate(list))
                .Do(_ => Log.Debug("GetFriendsListAsync loaded"))
                .Subscribe();
        }

        public IObservableCache<SteamPlayerDto,string> AllPlayers { get; }

        public bool IsValidId(string id)
        {
            if (id != null &&
                id.Length == 17 && 
                long.TryParse(id, out var _)) return true;
            return false;
        }

        public async Task ReloadAsync()
        {
            Log.Debug("PlayersService ReloadAsync");
            var friends = await GetFriendsListAsync();
            _itemsSource.Clear();
            _itemsSource.AddOrUpdate(friends);
            Log.Debug($"PlayersService ReloadAsync {friends.Count}");
        }

        public async Task AddFriendAsync(string id)
        {
            if (!IsValidId(id)) return;

            var savedPlayers = await GetFriendsListAsync();
            var player = savedPlayers.FirstOrDefault(x => x.SteamId == id);
            if (player == null)
            {
                try
                {
                    var players = await GetSteamPlayersAsync(id);
                    player = players.FirstOrDefault();
                    if (player != null)
                    {
                        savedPlayers.Add(player);
                        await SaveFriendsListAsync(savedPlayers);
                        _itemsSource.AddOrUpdate(player);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
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
                players = await GetSteamPlayersAsync(ids);
                
                await SaveFriendsListAsync(players);
                _itemsSource.Edit(u =>
                {
                    u.AddOrUpdate(players);
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        private async Task<List<SteamPlayerDto>> GetSteamPlayersAsync(string ids)
        {
            var url = $"https://{Aoe2ApiClient.BaseUrl}/api/v3/players?ids={ids}";
            return await _httpClient.GetFromJsonAsync<List<SteamPlayerDto>>(url);
        }

        private async Task SaveFriendsListAsync(IEnumerable<SteamPlayerDto> players)
        {
            await _storageHelper.CreateFileAsync("favoritePlayers.json", players);
        }

        public async Task<List<SteamPlayerDto>> GetFriendsListAsync()
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
