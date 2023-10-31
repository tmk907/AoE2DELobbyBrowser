using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AoE2DELobbyBrowser.Api;
using Serilog;

namespace AoE2DELobbyBrowser.Services
{
    public interface IPlayersService
    {
        Task AddFriendAsync(string id);
        Task<List<SteamPlayerDto>> GetFriendsListAsync();
        bool IsValidId(string id);
        Task RemoveFriendAsync(string id);
        Task UpdatePlayersFromSteamAsync();
    }

    public class PlayersService : IPlayersService
    {
        private const string FileName = "favoritePlayers.json";
        private ApplicationDataStorageHelper _storageHelper;

        private readonly IApiClient _apiClient;

        public PlayersService(IApiClient apiClient)
        {
            _apiClient = apiClient;
            _storageHelper = new ApplicationDataStorageHelper(new AppDataJsonSerializer());
        }

        public bool IsValidId(string id)
        {
            if (id != null &&
                id.Length == 17 && 
                long.TryParse(id, out var _)) return true;
            return false;
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
                    var players = await _apiClient.GetSteamPlayersAsync(id);
                    player = players.FirstOrDefault();
                    if (player != null)
                    {
                        savedPlayers.Add(player);
                        await SaveFriendsListAsync(savedPlayers);
                        // TODO
                        //_playersCache.AddOrUpdate(player);
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
                // TODO
                //_playersCache.Remove(toRemove);
            }
        }

        public async Task UpdatePlayersFromSteamAsync()
        {
            try
            {
                var players = await GetFriendsListAsync();
                if (players.Count == 0) return;

                var ids = string.Join(',', players.Select(x => x.SteamId));
                players = await _apiClient.GetSteamPlayersAsync(ids);
                
                await SaveFriendsListAsync(players);
                // TODO
                //_playersCache.Edit(u =>
                //{
                //    u.AddOrUpdate(players);
                //});
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        private async Task SaveFriendsListAsync(IEnumerable<SteamPlayerDto> players)
        {
            await _storageHelper.CreateFileAsync(FileName, players);
        }

        public async Task<List<SteamPlayerDto>> GetFriendsListAsync()
        {
            try
            {
                return await _storageHelper.ReadFileAsync(FileName, new List<SteamPlayerDto>());
            }
            catch (FileNotFoundException)
            {
                await _storageHelper.CreateFileAsync(FileName, new List<SteamPlayerDto>());
                return new List<SteamPlayerDto>();
            }
        }
    }
}
