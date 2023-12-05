using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;

namespace AoE2DELobbyBrowserAvalonia.Desktop;

public class WindowsConfiguration : IConfiguration
{
    public WindowsConfiguration()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        ApiBaseUrl = configuration["ApiBaseUrl"]!;

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
    public string ApiBaseUrl { get; }

    public string AppDisplayName { get; }
    public Version AssemblyVersion { get; }
    public string InformationVersion { get; }
}