using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AoE2DELobbyBrowser.Models
{
    public class LobbySettings : ReactiveObject
    {
        [Reactive]
        public string Query { get; set; }
        [Reactive]
        public string Exclude { get; set; }
        [Reactive]
        public bool IsAutoRefreshEnabled { get; set; }
        [Reactive]
        public int Interval { get; set; }
        [Reactive]
        public string SelectedGameType { get; set; }
        [Reactive]
        public string SelectedGameSpeed { get; set; }
        [Reactive]
        public string SelectedMapType { get; set; }
        [Reactive]
        public bool ShowNotifications { get; set; }

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

        private string PlayerQuery { get; set; }
        private bool IsPlayerSearchEnabled { get; set; }
    }
}
