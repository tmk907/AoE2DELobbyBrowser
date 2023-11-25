using Avalonia.Controls;

namespace AoE2DELobbyBrowserAvalonia.Views;

public partial class PlayersPopupControl : UserControl
{
    public PlayersPopupControl()
    {
        InitializeComponent();
    }

    private void Grid_Tapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        e.Handled = true;
    }
}