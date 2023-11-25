using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowserAvalonia.Api
{
    public interface IApiClient
    {
        Task<List<LobbyDto>> GetAllLobbiesAsync(CancellationToken cancellationToken);
        Task<List<SteamPlayerDto>> GetSteamPlayersAsync(string ids);
    }
}