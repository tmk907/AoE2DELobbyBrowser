using AoE2DELobbyBrowserAvalonia.Models;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;

namespace AoE2DELobbyBrowserAvalonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        Settings = new LobbySettings();
        Loading = false;
        LobbyListViewModel = new LobbyListViewModel();
    }

    public AsyncRelayCommand RefreshCommand { get; }
    public AsyncRelayCommand<Player> AddFriendCommand { get; }

    public LobbySettings Settings { get; private set; }

    public LobbyListViewModel LobbyListViewModel { get; }

    public bool Loading { get; }

    public int OnlineCount { get; }

    public List<string> GameTypes { get; } = new GameType().GetAll();
    public List<string> GameSpeeds { get; } = new GameSpeed().GetAll();
    public List<string> MapTypes { get; } = new MapType().GetAll();
}
