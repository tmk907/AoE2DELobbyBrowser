// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Windows.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AoE2DELobbyBrowser
{
    public sealed partial class PlayersPopupControl : UserControl
    {
        public PlayersPopupControl()
        {
            this.InitializeComponent();
        }

        public ICommand AddFriendCommand
        {
            get { return (ICommand)GetValue(AddFriendCommandProperty); }
            set { SetValue(AddFriendCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddFriendCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddFriendCommandProperty =
            DependencyProperty.Register("AddFriendCommand", typeof(ICommand), typeof(PlayersPopupControl), new PropertyMetadata(0));

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
