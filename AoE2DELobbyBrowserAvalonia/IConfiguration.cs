namespace AoE2DELobbyBrowserAvalonia
{
    public interface IConfiguration
    {
        IAppConfig AppConfig { get; }
        string AppDataFolder { get; }
        string LogsFolder { get; }
    }
}