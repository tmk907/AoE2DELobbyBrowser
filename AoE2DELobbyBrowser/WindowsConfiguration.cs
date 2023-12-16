using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;
using Windows.ApplicationModel;

namespace AoE2DELobbyBrowser;

public class WinUI3Configuration : Core.IConfiguration
{
    public WinUI3Configuration()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Package.Current.InstalledLocation.Path)
            .AddJsonFile("appsettings.json")
            .Build();

        ApiBaseUrl = configuration["ApiBaseUrl"]!;

        var assembly = Assembly.GetExecutingAssembly();
        AppDisplayName = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? "";

        var version = Package.Current.Id.Version;
        AssemblyVersion = new Version(version.Major, version.Minor, version.Build, version.Revision);
        InformationVersion = Package.Current.DisplayName;

        AppDataFolder = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
        LogsFolder = Path.Combine(AppDataFolder, "logs");
    }

    public string AppDataFolder { get; }
    public string LogsFolder { get; }
    public string ApiBaseUrl { get; }

    public string AppDisplayName { get; }
    public Version AssemblyVersion { get; }
    public string InformationVersion { get; }
}