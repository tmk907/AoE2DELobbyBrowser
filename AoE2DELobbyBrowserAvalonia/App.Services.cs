using AoE2DELobbyBrowser.Services;
using AoE2DELobbyBrowserAvalonia.Api;
using AoE2DELobbyBrowserAvalonia.Services;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AoE2DELobbyBrowserAvalonia
{
    public partial class App
    {
        private IPlatformServices? _platformServices;

        public IServiceProvider Services { get; private set; }

        public void AddPlatformServices(IPlatformServices platformServices)
        {
            _platformServices = platformServices;
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IClipboard>(s =>
            {
                if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    return desktop.MainWindow.Clipboard;
                }
                return null;
            });
            services.AddSingleton<IApiClient>(_ => new Aoe2ApiClient());
            services.AddSingleton<AppSettingsService>();
            services.AddSingleton<ApplicationDataStorageHelper>();
            services.AddSingleton<PlayersService>();
            services.AddSingleton<CountryService>();

            _platformServices?.Register(services);

            return services.BuildServiceProvider();
        }
    }
}
