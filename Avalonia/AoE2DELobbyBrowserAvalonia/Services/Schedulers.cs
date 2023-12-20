using AoE2DELobbyBrowser.Core.Services;
using Avalonia.ReactiveUI;
using System.Reactive.Concurrency;

namespace AoE2DELobbyBrowserAvalonia.Services
{
    public class Schedulers : ISchedulers
    {
        public IScheduler UIScheduler => AvaloniaScheduler.Instance;

        public IScheduler BackgroundScheduler => Scheduler.Default;
    }
}
