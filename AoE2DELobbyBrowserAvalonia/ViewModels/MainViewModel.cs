using AoE2DELobbyBrowser.Services;
using AoE2DELobbyBrowserAvalonia.Models;
using AoE2DELobbyBrowserAvalonia.Services;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowserAvalonia.ViewModels;

public interface IMainViewModel
{
    IRelayCommand NavigateToSettingsCommand { get; }
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
    private readonly AppSettingsService _appSettingsService;
    public MainViewModel()
    {
        _appSettingsService = Ioc.Default.GetRequiredService<AppSettingsService>();
        Settings = _appSettingsService.GetLobbySettings();
        Settings.PropertyChanged += Settings_PropertyChanged;

        Loading = false;
        LobbyListViewModel = new LobbyListViewModel();
    }

    private void Settings_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        _appSettingsService.SaveLobbySettings(Settings);   
    }

    public LobbySettings Settings { get; }

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

    [RelayCommand]
    private void NavigateToSettings()
    {
        WeakReferenceMessenger.Default.Send(new NavigateToMessage(typeof(SettingsViewModel)));
    }
}
