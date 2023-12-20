using AoE2DELobbyBrowser.Core.Services;
using ReactiveUI;
using System.Reactive.Concurrency;

namespace AoE2DELobbyBrowser.Services
{
    public class Schedulers : ISchedulers
    {
        public IScheduler UIScheduler => RxApp.MainThreadScheduler;

        public IScheduler BackgroundScheduler => RxApp.TaskpoolScheduler;
    }
}
