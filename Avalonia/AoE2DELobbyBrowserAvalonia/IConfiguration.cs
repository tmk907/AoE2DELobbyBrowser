using System;

namespace AoE2DELobbyBrowserAvalonia
{
    public interface IConfiguration
    {
        IAppConfig AppConfig { get; }
        string AppDataFolder { get; }
        string LogsFolder { get; }
        string AppDisplayName { get; }
        Version AssemblyVersion { get; }
        string InformationVersion { get; }
    }
}