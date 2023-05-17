using AoE2DELobbyBrowser.Models;
using DynamicData;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowser.Api
{
    public interface IApiClient
    {
        IObservable<IChangeSet<Lobby, string>> Connect();
        void Dispose();
        Task Refresh(CancellationToken cancellationToken);
    }
}