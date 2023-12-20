using AoE2DELobbyBrowser.Core.Models;
using AoE2DELobbyBrowser.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace AoE2DELobbyBrowser.Core.ViewModels;

public interface ILobbyListViewModel
{
    IRelayCommand<LobbyVM> SelectLobbyCommand { get; }
    LobbyVM? SelectedLobby { get; }
    ReadOnlyObservableCollection<LobbyVM> Lobbies { get; }
}

public partial class LobbyListViewModel : ObservableObject, ILobbyListViewModel
{
    private readonly IScheduler _uiScheduler;

    public LobbyListViewModel()
    {
        var myAdaptor = new MySortedObservableCollectionAdaptor();
        var lobbyService = Ioc.Default.GetRequiredService<LobbyService>();
        _uiScheduler = Ioc.Default.GetRequiredService<ISchedulers>().UIScheduler;

        lobbyService.FilteredLobbyChanges
            .Sort(SortExpressionComparer<LobbyVM>.Ascending(t => t.Name))
            .Do(x => Log.Debug("LobbiesVM {0}", x.Count))
            .ObserveOn(_uiScheduler)
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

    [ObservableProperty]
    private LobbyVM? _selectedLobby;

    [RelayCommand]
    private void SelectLobby(LobbyVM lobby)
    {
        SelectedLobby = lobby;
    }

    private readonly ReadOnlyObservableCollection<LobbyVM> _lobbies;
    public ReadOnlyObservableCollection<LobbyVM> Lobbies => _lobbies;
}
