using AoE2DELobbyBrowser.Core.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AoE2DELobbyBrowserAvalonia.Views;

public partial class LobbyListControl : UserControl
{
    public LobbyListControl()
    {
        InitializeComponent();
        Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var vm = (DataContext as LobbyListViewModel);
        vm.SelectedLobby = null;
    }

    private void Grid_Tapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        var vm = (DataContext as LobbyListViewModel);
        vm.SelectedLobby = null;
    }
}