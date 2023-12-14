using AoE2DELobbyBrowser.Core.Models;
using DynamicData.Binding;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.Json;

namespace AoE2DELobbyBrowser.Core.Services
{
    public class AppSettingsService : IDisposable
    {
        private readonly string _settingsPath;
        private readonly AppSettings _settings;
        private readonly CompositeDisposable Disposal = new CompositeDisposable();

        public AppSettingsService(IConfiguration configuration)
        {
            _settingsPath = Path.Combine(configuration.AppDataFolder, "settings.json");

            _settings = ReadFile(_settingsPath, GetDefaultSettings);

            _settings.WhenAnyPropertyChanged()
                .Do(_ => Log.Debug($"Settings changed"))
                .Do(lobbySettings => SaveFile(_settingsPath, _settings))
                .Subscribe()
                .DisposeWith(Disposal);

            _settings.LobbySettings.WhenAnyPropertyChanged()
                .Do(_ => Log.Debug($"LobbySettings changed"))
                .Do(lobbySettings => SaveFile(_settingsPath, _settings))
                .Subscribe()
                .DisposeWith(Disposal);
        }

        public void Dispose()
        {
            Disposal.Dispose();
        }

        public AppSettings AppSettings => _settings;

        private AppSettings GetDefaultSettings()
        {
            return new AppSettings
            {
                JoinLinkType = JoinLinkEnum.Aoe2de,
                LobbySettings = new LobbySettings
                {
                    Interval = 10,
                    IsAutoRefreshEnabled = true,
                    Query = "",
                    Exclude = "",
                    SelectedGameSpeed = new GameSpeed().GetAll().First(),
                    SelectedGameType = new GameType().GetAll().First(),
                    SelectedMapType = new MapType().GetAll().First(),
                    ShowNotifications = true,
                },
                NewLobbyHighlightTime = TimeSpan.FromSeconds(30),
                Separator = "/"
            };
        }
        private void SaveFile(string path, object data)
        {
            using var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            JsonSerializer.Serialize(fileStream, data);
        }

        private T ReadFile<T>(string path, Func<T> createDefaultValue)
        {
            try
            {
                using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                var result = JsonSerializer.Deserialize<T>(fileStream);
                if (result == null)
                {
                    return createDefaultValue();
                }
                return result;
            }
            catch (Exception)
            {
                return createDefaultValue();
            }
        }
    }
}
