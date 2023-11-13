using AoE2DELobbyBrowserAvalonia.Models;
using AoE2DELobbyBrowserAvalonia.Services;
using Avalonia.ReactiveUI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace AoE2DELobbyBrowserAvalonia.ViewModels;

public interface ILobbyListViewModel
{
    IRelayCommand<Lobby> SelectLobbyCommand { get; }
    Lobby? SelectedLobby { get; }
    ReadOnlyObservableCollection<Lobby> Lobbies { get; }
}

public partial class LobbyListViewModel : ViewModelBase, ILobbyListViewModel
{
    public LobbyListViewModel()
    {
        var myAdaptor = new MySortedObservableCollectionAdaptor();
        var lobbyService = Ioc.Default.GetRequiredService<LobbyService>();
        lobbyService.FilteredLobbyChanges
            .Sort(SortExpressionComparer<Lobby>.Ascending(t => t.Name))
            .Do(x => Log.Debug("LobbiesVM {0}", x.Count))
            .ObserveOn(AvaloniaScheduler.Instance)
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
    private Lobby? _selectedLobby;

    [RelayCommand]
    private void SelectLobby(Lobby lobby)
    {
        SelectedLobby = lobby;
    }

    private readonly ReadOnlyObservableCollection<Lobby> _lobbies;
    public ReadOnlyObservableCollection<Lobby> Lobbies => _lobbies;
}
