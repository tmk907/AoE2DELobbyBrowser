using AoE2DELobbyBrowserAvalonia.Models;
using System;
using System.Collections.ObjectModel;

namespace AoE2DELobbyBrowserAvalonia.ViewModels;

public interface ILobbyListViewModel
{
    ReadOnlyObservableCollection<Lobby> Lobbies { get; }
}

public partial class LobbyListViewModel : ViewModelBase, ILobbyListViewModel
{
    public LobbyListViewModel()
    {

    }

    public ReadOnlyObservableCollection<Lobby> Lobbies { get; }
}
