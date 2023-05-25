using AoE2DELobbyBrowser.Models;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace AoE2DELobbyBrowser
{
    public class LobbyListViewModel : IDisposable
    {
        public LobbyListViewModel() 
        {
            var myAdaptor = new MySortedObservableCollectionAdaptor();
            var filtered = App.LobbyService.FilteredLobbyChanges
                .Sort(SortExpressionComparer<Lobby>.Ascending(t => t.Name))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _lobbies, adaptor: myAdaptor)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(Disposal);
        }

        protected CompositeDisposable Disposal = new CompositeDisposable();
        public void Dispose()
        {
            Disposal.Dispose();
        }

        private readonly ReadOnlyObservableCollection<Lobby> _lobbies;
        public ReadOnlyObservableCollection<Lobby> Lobbies => _lobbies;
    }
}
