// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using AoE2DELobbyBrowser.Models;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AoE2DELobbyBrowser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FriendsPage : Page
    {
        public FriendsViewModel ViewModel { get; private set; }

        public FriendsPage()
        {
            this.InitializeComponent();
            ViewModel = new FriendsViewModel();
            Unloaded += MainPage_Unloaded;
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            Bindings.StopTracking();
            ViewModel.Dispose();
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.Send(new NavigateBackMessage());
        }

        private void FocusOnTextBox(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var textBox = args.Element as TextBox;
            if (textBox != null)
            {
                textBox.Focus(FocusState.Programmatic);
            }
            args.Handled = true;
        }

        private void FocusOnPage(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Escape || e.Key == Windows.System.VirtualKey.Enter)
            {
                this.Focus(FocusState.Programmatic);
            }
        }

        private void ShowPlayersPopup(object sender, TappedRoutedEventArgs e)
        {
            PlayersPopup.Visibility = Visibility.Visible;
            var friend = (e.OriginalSource as FrameworkElement).DataContext as Friend;
            PlayersPopup.DataContext = friend.Lobby;
        }

        private void ClosePlayersPopup(object sender, TappedRoutedEventArgs e)
        {
            PlayersPopup.Visibility = Visibility.Collapsed;
            PlayersPopup.DataContext = null;
        }
    }
}
