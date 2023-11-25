using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AoE2DELobbyBrowserAvalonia.Views;

public partial class MainView : UserControl, IRecipient<KeyboardShortcutMessage>
{
    public MainView()
    {
        InitializeComponent();

        WeakReferenceMessenger.Default.Register<KeyboardShortcutMessage>(this);
    }

    public void Receive(KeyboardShortcutMessage message)
    {
        switch(message)
        {
            case { Key: Key.F, Modifier: KeyModifiers.Control }:
                this.FindControl<TextBox>("searchTextBox")?.Focus(NavigationMethod.Pointer);
                break;
            case { Key: Key.G, Modifier: KeyModifiers.Control }:
                this.FindControl<TextBox>("excludeTextBox")?.Focus();
                break;
            case { Key: Key.Q, Modifier: KeyModifiers.Control }:
                var c = this.FindControl<ComboBox>("gameTypesCombobox");
                c?.Focus();
                break;
            default:
                break;
        }
    }

    private void UnFocusOnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape || e.Key == Key.Enter) 
        {
            TopLevel.GetTopLevel(this)?.FocusManager?.ClearFocus();
        }
    }
}
