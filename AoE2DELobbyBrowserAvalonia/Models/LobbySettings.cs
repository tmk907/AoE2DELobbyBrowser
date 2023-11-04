using CommunityToolkit.Mvvm.ComponentModel;

namespace AoE2DELobbyBrowserAvalonia.Models
{
    public partial class LobbySettings : ObservableObject
    {
        [ObservableProperty]
        private string query;
        [ObservableProperty]
        private string exclude;
        [ObservableProperty]
        private bool isAutoRefreshEnabled;
        [ObservableProperty]
        private int interval;
        [ObservableProperty]
        private string selectedGameType;
        [ObservableProperty]
        private string selectedGameSpeed;
        [ObservableProperty]
        private string selectedMapType;
        [ObservableProperty]
        private bool showNotifications;
        [ObservableProperty]
        private string playerQuery;
        [ObservableProperty]
        private bool isPlayerSearchEnabled;

        public void Update(LobbySettings settings)
        {
            Exclude = settings.Exclude;
            Interval = settings.Interval;
            IsAutoRefreshEnabled = settings.IsAutoRefreshEnabled;
            IsPlayerSearchEnabled = settings.IsPlayerSearchEnabled;
            PlayerQuery = settings.PlayerQuery;
            Query = settings.Query;
            SelectedGameSpeed = settings.SelectedGameSpeed;
            SelectedGameType = settings.SelectedGameType;
            SelectedMapType = settings.SelectedMapType;
            ShowNotifications = settings.ShowNotifications;
        }
    }
}
