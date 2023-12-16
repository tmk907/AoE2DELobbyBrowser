using Microsoft.UI.Xaml.Controls;
using AoE2DELobbyBrowser.Core.ViewModels;
using AoE2DELobbyBrowser.Core.Services;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AoE2DELobbyBrowser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private SettingsViewModel ViewModel { get; }

        public SettingsPage()
        {
            this.InitializeComponent();
            ViewModel = new SettingsViewModel();
        }

        public bool IsAoe2deLink => ViewModel.JoinLink == JoinLinkEnum.Aoe2de;
        public bool IsSteamLink => ViewModel.JoinLink == JoinLinkEnum.Steam;

        public string Version
        {
            get
            {
                var version = Windows.ApplicationModel.Package.Current.Id.Version;
                return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            }
        }

        private void Aoe2de_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.JoinLink = JoinLinkEnum.Aoe2de;
        }

        private void Steam_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.JoinLink = JoinLinkEnum.Steam;
        }
    }
}
