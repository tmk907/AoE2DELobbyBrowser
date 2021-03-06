using DynamicData;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowser.Api
{
    internal interface IApiClient
    {
        IObservable<IChangeSet<Lobby, string>> Connect();
        void Dispose();
        Task Refresh(CancellationToken cancellationToken);
    }
}