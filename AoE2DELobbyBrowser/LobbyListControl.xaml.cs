using AoE2DELobbyBrowser.Core.Models;
using AoE2DELobbyBrowser.Core.ViewModels;
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

        public ILobbyListViewModel ViewModel
        {
            get { return (ILobbyListViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ILobbyListViewModel), typeof(LobbyListControl), new PropertyMetadata(0));

        private void NumPlayersTapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var lobby = (e.OriginalSource as FrameworkElement).DataContext as LobbyVM;
            NumPlayersTappedEvent?.Invoke(this, lobby);
        }

        public event EventHandler<LobbyVM> NumPlayersTappedEvent;
    }
}
