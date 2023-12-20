using System.Reactive.Concurrency;

namespace AoE2DELobbyBrowser.Core.Services
{
    public interface ISchedulers
    {
        IScheduler UIScheduler { get; }
        IScheduler BackgroundScheduler { get; }
    }
}
