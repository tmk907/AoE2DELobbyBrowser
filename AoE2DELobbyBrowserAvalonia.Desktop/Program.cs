using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Config.Net;
using Microsoft.Toolkit.Uwp.Notifications;
using Serilog;

namespace AoE2DELobbyBrowserAvalonia.Desktop;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var config = new ConfigurationBuilder<IAppConfig>()
               .UseJsonFile(WindowsConfiguration.GetConfigFilePath())
               .Build();
        var platformConfig = new WindowsConfiguration(config);

        Log.Logger = new LoggerConfiguration()
#if DEBUG
           .MinimumLevel.Debug()
           .WriteTo.Debug()
#else
               .MinimumLevel.Information()
#endif
          .WriteTo.File(Path.Combine(platformConfig.LogsFolder, "logs.txt"), rollingInterval: RollingInterval.Day)
          .CreateLogger();

        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

        var isToastActivated = ToastNotificationManagerCompat.WasCurrentProcessToastActivated();
        NotificationManager = new WindowsNotificationManager();

        Log.Information("App started {0} , isToastActivated {1}", DateTime.Now, isToastActivated);
        Log.Information("{0}", AppContext.BaseDirectory);
        Log.Information("{0}", platformConfig.AppDataFolder);
        Log.Information("{0}", platformConfig.LogsFolder);
        Log.Information("{0}", platformConfig.AppConfig.ApplicationName);

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static WindowsNotificationManager NotificationManager { get; private set; }

    private static void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        Log.Error("TaskScheduler error {0}", e.Exception);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .AfterSetup(appBuilder =>
            {
                var app = (App)appBuilder.Instance!;
                app.AddPlatformServices(new PlatformServices());
            });
}
