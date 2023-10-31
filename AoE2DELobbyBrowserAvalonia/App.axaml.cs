using AoE2DELobbyBrowser.Api;
using AoE2DELobbyBrowser.Services;
using AoE2DELobbyBrowserAvalonia.ViewModels;
using AoE2DELobbyBrowserAvalonia.Views;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
//using Live.Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Diagnostics;

namespace AoE2DELobbyBrowserAvalonia;

public interface IPlatformServices
{
    void Register(IServiceCollection services);
}

public partial class App : Application //, ILiveView
{
    private IPlatformServices? _platformServices;

    public IServiceProvider Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
    public static string LogsFolderPath { get; }

    //public static IPlayersService PlayersService { get; private set; }
    public static IApiClient ApiClient { get; private set; }
    public static CountryService CountryService { get; private set; }

    public static IClipboard Clipboard { get; private set; }

    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        Services = ConfigureServices();
        Ioc.Default.ConfigureServices(Services);

        if (Debugger.IsAttached || IsProduction())
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainViewModel()
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                singleViewPlatform.MainView = new MainView
                {
                    DataContext = new MainViewModel()
                };
            }
        }
        else
        {
            // Hot Reload
            //var window = new LiveViewHost(this, Log.Information);
            //window.StartWatchingSourceFilesForHotReloading();
            //window.Show();
        }

        base.OnFrameworkInitializationCompleted();
    }

    public void AddPlatformServices(IPlatformServices platformServices)
    {
        _platformServices = platformServices;
    }

    private IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        _platformServices?.Register(services);

        return services.BuildServiceProvider();
    }

    private static bool IsProduction()
    {
#if DEBUG
        return false;
#else
        return true;
#endif
    }

    public object CreateView(Window window)
    {
        window.DataContext = new MainViewModel();
        return new MainView();
    }
}
