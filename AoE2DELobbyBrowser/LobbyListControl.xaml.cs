using AoE2DELobbyBrowser.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AoE2DELobbyBrowser
{
    public sealed partial class LobbyListControl : UserControl
    {
        public LobbyListControl()
        {
            this.InitializeComponent();
        }

        public LobbyListViewModel ViewModel
        {
            get { return (LobbyListViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(LobbyListViewModel), typeof(LobbyListControl), new PropertyMetadata(0));

        private void NumPlayersTapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var lobby = (e.OriginalSource as FrameworkElement).DataContext as Lobby;
            NumPlayersTappedEvent?.Invoke(this, lobby);
        }

        public event EventHandler<Lobby> NumPlayersTappedEvent;
    }
}
