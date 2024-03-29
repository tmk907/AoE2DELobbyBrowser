﻿using AoE2DELobbyBrowser.Core;
using AoE2DELobbyBrowser.Core.Models;
using AoE2DELobbyBrowser.Core.ViewModels;
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
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; private set; }

        public MainPage()
        {
            this.InitializeComponent();
            ViewModel = new MainViewModel();
            Unloaded += MainPage_Unloaded;
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            Bindings.StopTracking();
            ViewModel.Dispose();
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

        private void NavigateToSettigns_Click(object sender, RoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.Send(new NavigateToMessage(typeof(SettingsPage)));
        }

        private void ShowPlayersPopup(object sender, TappedRoutedEventArgs e)
        {
            var lobby = (e.OriginalSource as FrameworkElement).DataContext as LobbyVM;
            ShowPlayersPopup(lobby);
        }

        private void ClosePlayersPopup(object sender, TappedRoutedEventArgs e)
        {
            PlayersPopup.Visibility = Visibility.Collapsed;
            PlayersPopup.DataContext = null;
        }

        private void NavigateToFriends_Click(object sender, RoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.Send(new NavigateToMessage(typeof(FriendsPage)));
        }

        private void ShowPlayersPopup(LobbyVM lobby)
        {
            PlayersPopup.Visibility = Visibility.Visible;
            PlayersPopup.DataContext = lobby;
        }

        private void OnNumPlayersTapped(object sender, LobbyVM lobby)
        {
            ShowPlayersPopup(lobby);
        }
    }
}
