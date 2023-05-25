using AoE2DELobbyBrowser.Models;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowser.Api
{
    public interface IApiClient
    {
        IObservable<IChangeSet<Lobby, string>> LobbyChanges { get; }
        void Dispose();
        Task<List<SteamPlayerDto>> GetSteamPlayersAsync(string ids);
        Task RefreshAllLobbiesAsync(CancellationToken cancellationToken);
    }
}