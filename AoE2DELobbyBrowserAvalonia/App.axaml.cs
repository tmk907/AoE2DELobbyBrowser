using AoE2DELobbyBrowserAvalonia.Api;
using AoE2DELobbyBrowserAvalonia.Services;
using AoE2DELobbyBrowserAvalonia.ViewModels;
using AoE2DELobbyBrowserAvalonia.Views;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
//using Live.Avalonia;
using System.Diagnostics;

namespace AoE2DELobbyBrowserAvalonia;

public partial class App : Application //, ILiveView
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

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
            //else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            //{
            //    singleViewPlatform.MainView = new MainView
            //    {
            //        DataContext = new MainViewModel()
            //    };
            //}
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
