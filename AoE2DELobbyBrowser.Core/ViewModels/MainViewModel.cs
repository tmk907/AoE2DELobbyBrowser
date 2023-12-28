using AoE2DELobbyBrowser.Core.Models;
using AoE2DELobbyBrowser.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowser.Core.ViewModels;

public interface IMainViewModel
{
    IRelayCommand NavigateToSettingsCommand { get; }
    IAsyncRelayCommand RefreshCommand { get; }
    IRelayCommand NavigateToFriendsCommand { get; }

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
    private readonly LobbyService _lobbyService;
    private readonly IScheduler _uiScheduler;

    public MainViewModel()
    {
        _lobbyService = Ioc.Default.GetRequiredService<LobbyService>();
        _appSettingsService = Ioc.Default.GetRequiredService<AppSettingsService>();
        _uiScheduler = Ioc.Default.GetRequiredService<ISchedulers>().UIScheduler;

        Settings = _appSettingsService.AppSettings.LobbySettings;

        LobbyListViewModel = new LobbyListViewModel();

        _lobbyService.IsLoading
                .DistinctUntilChanged()
                .ObserveOn(_uiScheduler)
                .Do(x => Loading = x)
                .Subscribe()
                .DisposeWith(Disposal);

        _lobbyService.FriendsOnline
            .ObserveOn(_uiScheduler)
            .Do(x=>OnlineCount = x)
            .Subscribe()
            .DisposeWith(Disposal);
    }

    protected CompositeDisposable Disposal = new CompositeDisposable();
    public void Dispose()
    {
        Log.Debug("Dispose MainViewModel");
        Disposal.Dispose();
    }

    public LobbySettings Settings { get; }

    public ILobbyListViewModel LobbyListViewModel { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanRefresh))]
    [NotifyCanExecuteChangedFor(nameof(RefreshCommand))]
    private bool _loading;

    [ObservableProperty]
    private int _onlineCount;

    public List<string> GameTypes { get; } = new GameType().GetAll();
    public List<string> GameSpeeds { get; } = new GameSpeed().GetAll();
    public List<string> MapTypes { get; } = new MapType().GetAll();

    public bool CanRefresh => !Loading;

    [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanRefresh))]
    private async Task RefreshAsync(CancellationToken ct)
    {
        await _lobbyService.RefreshAsync(ct);
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        WeakReferenceMessenger.Default.Send(new NavigateToMessage(typeof(SettingsViewModel)));
    }

    [RelayCommand]
    private void NavigateToFriends()
    {
        WeakReferenceMessenger.Default.Send(new NavigateToMessage(typeof(FriendsViewModel)));
    }
}
