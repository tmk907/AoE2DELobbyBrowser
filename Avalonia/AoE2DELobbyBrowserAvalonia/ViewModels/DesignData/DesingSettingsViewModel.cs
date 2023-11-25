using AoE2DELobbyBrowserAvalonia.Services;
using CommunityToolkit.Mvvm.Input;

namespace AoE2DELobbyBrowserAvalonia.ViewModels.DesignData
{
    public class DesingSettingsViewModel : ISettingsViewModel
    {
        public IAsyncRelayCommand OpenLogsFolderCommand { get; }

        public IAsyncRelayCommand RateAppCommand { get; }

        public IRelayCommand NavigateBackCommand { get; }

        public IAsyncRelayCommand<string> OpenUrlCommand { get; }

        public double NewLobbyHighlightTime { get; } = 15;

        public JoinLinkEnum JoinLink { get; } = JoinLinkEnum.Aoe2de;

        public string Separator { get; } = ";";

        public string Version { get; } = "0.1.0";
    }
}
