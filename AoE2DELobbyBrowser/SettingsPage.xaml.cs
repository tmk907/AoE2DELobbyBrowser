using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AoE2DELobbyBrowser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            newLobbyNumberBox.Value = AppSettings.NewLobbyHighlightTime.TotalSeconds;
        }

        public bool IsAoe2deLink => AppSettings.JoinLinkType == AppSettings.JoinLink.Aoe2de;
        public bool IsSteamLink => AppSettings.JoinLinkType == AppSettings.JoinLink.Steam;

        public string Version
        {
            get
            {
                var version = Windows.ApplicationModel.Package.Current.Id.Version;
                return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            }
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.Send(new NavigateBackMessage());
        }

        private void newLobbyNumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            double seconds = args.NewValue;
            if (double.IsNaN(seconds)) return;
            AppSettings.NewLobbyHighlightTime = TimeSpan.FromSeconds(seconds);
        }

        private void Aoe2de_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            AppSettings.JoinLinkType = AppSettings.JoinLink.Aoe2de;
        }

        private void Steam_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            AppSettings.JoinLinkType = AppSettings.JoinLink.Steam;
        }
    }
}
