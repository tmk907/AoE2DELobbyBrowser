using AoE2DELobbyBrowserAvalonia.Services;
using Config.Net;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace AoE2DELobbyBrowserAvalonia.Desktop;

public class PlatformServices : IPlatformServices
{
    public void Register(IServiceCollection services)
    {
        var config = new ConfigurationBuilder<IAppConfig>()
               .UseJsonFile(WindowsConfiguration.ConfigFileName)
               .Build();
        var platformConfig = new WindowsConfiguration(config);
        services.AddSingleton<IConfiguration>(platformConfig);

        var appSettings = new ConfigurationBuilder<IAppSettings>()
           .UseJsonFile(Path.Combine(platformConfig.AppDataFolder, "settings.json"))
           .Build();
        services.AddSingleton<IAppSettings>(appSettings);

        services.AddSingleton<ILauncherService, WindowsLauncherService>();
        services.AddSingleton<INotificationsService, NotificationsService>();
    }
}
