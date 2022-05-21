namespace AoE2DELobbyBrowser.Services
{
    public class LobbySettings
    {
        public string Query { get; set; }
        public bool IsAutoRefreshEnabled { get; set; }
        public int Interval { get; set; }
        public string SelectedGameType { get; set; }
        public string SelectedGameSpeed { get; set; }
        public string SelectedMapType { get; set; }
        public bool ShowNotifications { get; set; }
    }
}
