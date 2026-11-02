using AoE2DELobbyBrowser.Core.Services;
using ReactiveUI.Builder;
using ReactiveUI.Primitives.Concurrency;
using System;
using System.Reactive.Concurrency;

namespace AoE2DELobbyBrowser.Services
{
    public class Schedulers : ISchedulers
    {
        public Schedulers(IReactiveUIInstance reactiveUIInstance)
        {
            UIScheduler = new SequencerScheduler(reactiveUIInstance.MainThreadScheduler);
            BackgroundScheduler = new SequencerScheduler(reactiveUIInstance.TaskpoolScheduler);
        }

        public IScheduler UIScheduler { get; }

        public IScheduler BackgroundScheduler { get; }

        // ReactiveUI 26+ exposes its own ISequencer instead of System.Reactive's IScheduler,
        // so adapt it for the System.Reactive based code in Core.
        private sealed class SequencerScheduler : IScheduler
        {
            private readonly ISequencer _sequencer;

            public SequencerScheduler(ISequencer sequencer)
            {
                _sequencer = sequencer;
            }

            public DateTimeOffset Now => _sequencer.Now;

            public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action) =>
                _sequencer.Schedule(state, (_, s) => action(this, s));

            public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action) =>
                _sequencer.Schedule(state, dueTime, (_, s) => action(this, s));

            public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action) =>
                _sequencer.Schedule(state, dueTime, (_, s) => action(this, s));
        }
    }
}
