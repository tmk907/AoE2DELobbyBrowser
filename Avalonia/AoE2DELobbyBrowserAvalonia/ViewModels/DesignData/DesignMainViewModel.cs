using AoE2DELobbyBrowserAvalonia.Models;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;

namespace AoE2DELobbyBrowserAvalonia.ViewModels.DesignData;

public class DesignMainViewModel : IMainViewModel
{
    public DesignMainViewModel()
    {
        Settings = new LobbySettings()
        {
            IsAutoRefreshEnabled = true,
            ShowNotifications = true,
            Interval = 120,
            SelectedGameSpeed = GameSpeed.All,
            SelectedGameType = GameType.All,
            SelectedMapType = MapType.All,
        };
        LobbyListViewModel = new DesignLobbyListViewModel();
    }

    public IAsyncRelayCommand RefreshCommand { get; }

    public IAsyncRelayCommand<PlayerVM> AddFriendCommand { get; }

    public IRelayCommand NavigateToSettingsCommand { get; }

    public LobbySettings Settings { get; }

    public ILobbyListViewModel LobbyListViewModel { get; }

    public bool Loading => false;

    public int OnlineCount => 1;

    public List<string> GameTypes { get; } = new GameType().GetAll();
    public List<string> GameSpeeds { get; } = new GameSpeed().GetAll();
    public List<string> MapTypes { get; } = new MapType().GetAll();
}
