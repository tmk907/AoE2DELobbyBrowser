using System;
using System.IO;
using System.Reflection;

namespace AoE2DELobbyBrowserAvalonia.Desktop;

public class WindowsConfiguration : IConfiguration
{
    private const string ConfigFileName = "appsettings.json";

    public WindowsConfiguration(IAppConfig config)
    {
        AppConfig = config;

        var assembly = Assembly.GetExecutingAssembly();
        AppDisplayName = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? "";
        AssemblyVersion = assembly.GetName().Version ?? new Version(0, 1, 0);
        InformationVersion = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "";

        var localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        AppDataFolder = Path.Combine(localAppDataFolder, Assembly.GetExecutingAssembly().GetName().Name!);
        LogsFolder = Path.Combine(AppDataFolder, "logs");
    }

    public string AppDataFolder { get; }
    public string LogsFolder { get; }
    public IAppConfig AppConfig { get; }

    public string AppDisplayName { get; }
    public Version AssemblyVersion { get; }
    public string InformationVersion { get; }

    public static string GetConfigFilePath() => Path.Combine(AppContext.BaseDirectory, ConfigFileName);
}