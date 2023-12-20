using Microsoft.UI.Xaml;
using Serilog;
using System;
using CommunityToolkit.WinUI.Notifications;
using System.Runtime.InteropServices;
using Microsoft.UI.Dispatching;
using System.IO;
using AoE2DELobbyBrowser.Services;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using AoE2DELobbyBrowser.Core.Services;
using System.Reactive.Concurrency;
using AoE2DELobbyBrowser.Core;
using AoE2DELobbyBrowser.Core.Api;
using ReactiveUI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AoE2DELobbyBrowser
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            PlatformConfig = new WinUI3Configuration();

            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
                .WriteTo.Debug()
#else
               .MinimumLevel.Information()
#endif
               .WriteTo.File(Path.Combine(PlatformConfig.LogsFolder, "logs.txt"), rollingInterval: RollingInterval.Day)
               .CreateLogger();

            var isToastActivated = ToastNotificationManagerCompat.WasCurrentProcessToastActivated();
            ToastNotificationManagerCompat.OnActivated += ToastNotificationManagerCompat_OnActivated;

            Log.Information("App started {0} , isToastActivated {1}", DateTime.Now, isToastActivated);
            Log.Information("{0} ({1}) {2}", PlatformConfig.AppDisplayName, PlatformConfig.InformationVersion,
                PlatformConfig.AssemblyVersion);
            Log.Information("BaseDirectory: {0}", AppContext.BaseDirectory);
            Log.Information("AppDataFolder: {0}", PlatformConfig.AppDataFolder);
            Log.Information("LogsFolder: {0}", PlatformConfig.LogsFolder);

            var serviceProvider = ConfigureServices();
            Ioc.Default.ConfigureServices(serviceProvider);
        }

        private static WinUI3Configuration PlatformConfig;
        private static DispatcherQueue DispatcherQueue;


        private Window m_window;

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // Get the app-level dispatcher
            DispatcherQueue = DispatcherQueue.GetForCurrentThread();

            // If we weren't launched by a toast, launch our window like normal.
            // Otherwise if launched by a toast, our OnActivated callback will be triggered
            if (!ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
            {
                LaunchAndBringToForegroundIfNeeded();
            }
        }

        private async void ToastNotificationManagerCompat_OnActivated(ToastNotificationActivatedEventArgsCompat toastArgs)
        {
            ToastArguments args = ToastArguments.Parse(toastArgs.Argument);
            if (args.TryGetValue("JoinLink", out string link) && !string.IsNullOrEmpty(link))
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(link));
            }
            else if (args.TryGetValue("type", out string type) && type == "lobby notification")
            {
                // Use the dispatcher from the window if present, otherwise the app dispatcher
                var dispatcherQueue = m_window?.DispatcherQueue ?? App.DispatcherQueue;
                dispatcherQueue.TryEnqueue(delegate
                {
                    LaunchAndBringToForegroundIfNeeded();
                });
            }
        }

        private void LaunchAndBringToForegroundIfNeeded()
        {
            if (m_window == null)
            {
                m_window = new MainWindow();
                m_window.Activate();

                // Additionally we show using our helper, since if activated via a toast, it doesn't
                // activate the window correctly
                WindowHelper.ShowWindow(m_window);
            }
            else
            {
                WindowHelper.ShowWindow(m_window);
            }
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IAssetsLoader, AssetsLoader>();
            services.AddSingleton<IClipboardService, ClipboardService>();
            services.AddSingleton<ISchedulers, Schedulers>();

            services.AddSingleton<IApiClient, Aoe2ApiClient>();
            services.AddSingleton(services => new AppSettingsService(services.GetRequiredService<IConfiguration>()));
            services.AddSingleton<AppDataStorageHelper>();
            services.AddSingleton<IPlayersService,PlayersService>();
            services.AddSingleton<CountryService>();
            services.AddSingleton<LobbyService>();

            services.AddSingleton<IConfiguration>(PlatformConfig);
            services.AddSingleton<ILauncherService, WindowsLauncherService>();
            services.AddSingleton<INotificationsService, NotificationsService>();

            return services.BuildServiceProvider();
        }

        private static class WindowHelper
        {
            [DllImport("user32.dll")]
            private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool SetForegroundWindow(IntPtr hWnd);

            public static void ShowWindow(Window window)
            {
                // Bring the window to the foreground... first get the window handle...
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

                // Restore window if minimized... requires DLL import above
                ShowWindow(hwnd, 0x00000009);

                // And call SetForegroundWindow... requires DLL import above
                SetForegroundWindow(hwnd);
            }
        }
    }
}
