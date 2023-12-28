using AoE2DELobbyBrowser.Core;
using AoE2DELobbyBrowser.Core.Api;
using AoE2DELobbyBrowser.Core.Services;
using AoE2DELobbyBrowserAvalonia.Services;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reactive.Concurrency;

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
            services.AddSingleton<IAssetsLoader,AssetsLoader>();
            services.AddSingleton<IClipboardService,ClipboardService>();
            services.AddSingleton<ISchedulers, Schedulers>();

            services.AddSingleton<IApiClient,Aoe2ApiClient>();
            services.AddSingleton(services => new AppSettingsService(services.GetRequiredService<IConfiguration>()));
            services.AddSingleton<AppDataStorageHelper>();
            services.AddSingleton<IPlayersService, PlayersService>();
            services.AddSingleton<CountryService>();
            services.AddSingleton<LobbyService>();

            _platformServices?.Register(services);

            return services.BuildServiceProvider();
        }
    }
}
