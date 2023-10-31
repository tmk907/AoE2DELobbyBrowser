using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Microsoft.Extensions.DependencyInjection;
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
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        try
        {
            Log.Logger = new LoggerConfiguration()
#if DEBUG
               .MinimumLevel.Debug()
               .WriteTo.Debug()
#else
               .MinimumLevel.Information()
#endif
              //.WriteTo.File(Path.Combine(LogsFolderPath, "logs.txt"), rollingInterval: RollingInterval.Day)
              .CreateLogger();

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Log.Error("Application error {0}", ex);
        }
    }

    private static void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        Log.Error("TaskScheduler error {0}", e.Exception);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .AfterSetup(appBuilder => {
                var app = (App)appBuilder.Instance!;
                app.AddPlatformServices(new PlatformServices());
            });
}

public class PlatformServices : IPlatformServices
{
    public void Register(IServiceCollection services)
    {
        //services.AddSingleton<>();
    }
}