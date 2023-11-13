using System;
using System.IO;

namespace AoE2DELobbyBrowserAvalonia.Desktop;

public class WindowsConfiguration : IConfiguration
{
    public const string ConfigFileName = "appsettings.json";

    public WindowsConfiguration(IAppConfig config)
    {
        var localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        AppDataFolder = Path.Combine(localAppDataFolder, config.ApplicationName);
        LogsFolder = Path.Combine(AppDataFolder, "logs");
        AppConfig = config;
    }

    public string AppDataFolder { get; }
    public string LogsFolder { get; }
    public IAppConfig AppConfig { get; }
}