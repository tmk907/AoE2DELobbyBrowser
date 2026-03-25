using AoE2DELobbyBrowser.Core.Services;
using ReactiveUI.Builder;
using System.Reactive.Concurrency;

namespace AoE2DELobbyBrowser.Services
{
    public class Schedulers : ISchedulers
    {
        private readonly IReactiveUIInstance _reactiveUIInstance;

        public Schedulers(IReactiveUIInstance reactiveUIInstance)
        {
            _reactiveUIInstance = reactiveUIInstance;
        }

        public IScheduler UIScheduler => _reactiveUIInstance.MainThreadScheduler;

        public IScheduler BackgroundScheduler => _reactiveUIInstance.TaskpoolScheduler;
    }
}
