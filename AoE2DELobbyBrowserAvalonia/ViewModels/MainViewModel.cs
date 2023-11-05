using AoE2DELobbyBrowser.Services;
using AoE2DELobbyBrowserAvalonia.Models;
using AoE2DELobbyBrowserAvalonia.Services;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AoE2DELobbyBrowserAvalonia.ViewModels;

public interface IMainViewModel
{
    IAsyncRelayCommand RefreshCommand { get; }
    //IAsyncRelayCommand<Player> AddFriendCommand { get; }

    LobbySettings Settings { get; }

    ILobbyListViewModel LobbyListViewModel { get; }

    bool Loading { get; }

    int OnlineCount { get; }

    List<string> GameTypes { get; } 
    List<string> GameSpeeds { get; }
    List<string> MapTypes { get; }
}

public partial class MainViewModel : ViewModelBase, IMainViewModel
{
    public MainViewModel()
    {
        Settings = Ioc.Default.GetRequiredService<AppSettingsService>().GetLobbySettings();
            Loading = false;
            LobbyListViewModel = new LobbyListViewModel();
        
    }


    public LobbySettings Settings { get; private set; }

    public ILobbyListViewModel LobbyListViewModel { get; }

    public bool Loading { get; }

    public int OnlineCount { get; }

    public List<string> GameTypes { get; } = new GameType().GetAll();
    public List<string> GameSpeeds { get; } = new GameSpeed().GetAll();
    public List<string> MapTypes { get; } = new MapType().GetAll();

    [RelayCommand(AllowConcurrentExecutions =false)]
    private async Task RefreshAsync()
    {

    }

    [RelayCommand]
    private async Task AddFriendAsync()
    {

    }
}
