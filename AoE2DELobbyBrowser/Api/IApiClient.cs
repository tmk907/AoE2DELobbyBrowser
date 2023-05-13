using AoE2DELobbyBrowser.Models;
using DynamicData;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowser.Api
{
    public interface IApiClient
    {
        IObservableCache<Lobby, string> Items { get; }

        IObservable<IChangeSet<Lobby, string>> Connect();
        void Dispose();
        Task Refresh(CancellationToken cancellationToken);
    }
}