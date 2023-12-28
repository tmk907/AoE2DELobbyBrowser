using AoE2DELobbyBrowser.Core.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AoE2DELobbyBrowserAvalonia.Views;

public partial class FriendsView : UserControl, IRecipient<KeyboardShortcutMessage>
{
    public FriendsView()
    {
        InitializeComponent();
        Unloaded += OnUnloaded;

        WeakReferenceMessenger.Default.Register<KeyboardShortcutMessage>(this);
    }

    public void Receive(KeyboardShortcutMessage message)
    {
        switch (message)
        {
            case { Key: Key.F, Modifier: KeyModifiers.Control }:
                this.FindControl<TextBox>("searchTextBox")?.Focus(NavigationMethod.Pointer);
                break;
            default:
                break;
        }
    }

    private void OnUnloaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var vm = (DataContext as FriendsViewModel);
        vm.SelectedLobby = null;
    }

    private void Grid_Tapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        var vm = (DataContext as FriendsViewModel);
        vm.SelectedLobby = null;
    }

    private void UnFocusOnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape || e.Key == Key.Enter)
        {
            TopLevel.GetTopLevel(this)?.FocusManager?.ClearFocus();
        }
    }
}