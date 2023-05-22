using Microsoft.UI.Xaml;
using Serilog;
using System;
using CommunityToolkit.WinUI.Notifications;
using System.Runtime.InteropServices;
using Microsoft.UI.Dispatching;
using Windows.Storage;
using System.IO;
using AoE2DELobbyBrowser.Services;
using AoE2DELobbyBrowser.Api;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AoE2DELobbyBrowser
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static string LogsFolderPath => Path.Combine(ApplicationData.Current.LocalFolder.Path, "logs");

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
                .WriteTo.Debug()
#else
               .MinimumLevel.Information()
#endif
               .WriteTo.File(Path.Combine(LogsFolderPath, "logs.txt"), rollingInterval: RollingInterval.Day)
               .CreateLogger();

            ToastNotificationManagerCompat.OnActivated += ToastNotificationManagerCompat_OnActivated;
            ApiClient = new Aoe2ApiClient();
            PlayersService = new PlayersService();
            LobbyService = new LobbyService();
            CountryService = new CountryService();
        }

        public static DispatcherQueue DispatcherQueue { get; private set; }
        public static IPlayersService PlayersService { get; private set; }
        public static LobbyService LobbyService { get; private set; }
        public static IApiClient ApiClient { get; private set; }
        public static CountryService CountryService { get; private set; }

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
