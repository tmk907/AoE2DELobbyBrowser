﻿using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AoE2DELobbyBrowser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; }
        public MainPage()
        {
            this.InitializeComponent();
            ViewModel = new MainViewModel();
        }

        private void SearchBox_KeyboardAcceleratorInvoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            var textBox = args.Element as TextBox;
            if (textBox != null)
            {
                textBox.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
            }
            args.Handled = true;
        }

        private void SearchBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Escape || e.Key == Windows.System.VirtualKey.Enter)
            {
                this.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
            }
        }

        private void NavigateToSettigns_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.Send(new NavigateToMessage { Destination = typeof(SettingsPage) });
        }

        private void ShowPlayersPopup(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            PlayersPopup.Visibility = Visibility.Visible;
            var lobby = (e.OriginalSource as FrameworkElement).DataContext as Lobby;
            PlayersPopup.DataContext = lobby;
        }

        private void ClosePlayersPopup(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            PlayersPopup.Visibility = Visibility.Collapsed;
            PlayersPopup.DataContext = null;
        }
    }
}
