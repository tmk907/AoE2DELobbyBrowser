using AoE2DELobbyBrowserAvalonia.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AoE2DELobbyBrowserAvalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        RegisterKeyBinding(Key.F, KeyModifiers.Control);
        RegisterKeyBinding(Key.G, KeyModifiers.Control);
        RegisterKeyBinding(Key.Q, KeyModifiers.Control);

        DataContext = new MainWindowViewModel();

        Focusable = true;
        PointerPressed += MainWindow_PointerPressed;
    }

    private void MainWindow_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Focus();
    }

    private void RegisterKeyBinding(Key key, KeyModifiers keyModifiers)
    {
        this.KeyBindings.Add(new KeyBinding()
        {
            Command = SendKeyboardShortcutMessageCommand,
            CommandParameter = new KeyboardShortcutMessage(key, keyModifiers),
            Gesture = new KeyGesture(key, keyModifiers)
        });
    }

    [RelayCommand]
    private void SendKeyboardShortcutMessage(KeyboardShortcutMessage? message)
    {
        if (message == null) return;
        WeakReferenceMessenger.Default.Send(message);
    }
}
