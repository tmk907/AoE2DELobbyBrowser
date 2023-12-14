using AoE2DELobbyBrowser.Core;
using AoE2DELobbyBrowser.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AoE2DELobbyBrowserAvalonia.Desktop;

public class PlatformServices : IPlatformServices
{
    public void Register(IServiceCollection services)
    {
        services.AddSingleton<IConfiguration>(Program.PlatformConfig);
        services.AddSingleton<ILauncherService, WindowsLauncherService>();
        services.AddSingleton<INotificationsService, NotificationsService>();
    }
}
