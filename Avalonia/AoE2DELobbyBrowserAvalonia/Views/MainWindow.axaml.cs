using AoE2DELobbyBrowser.Core;
using AoE2DELobbyBrowserAvalonia.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AoE2DELobbyBrowserAvalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var config = Ioc.Default.GetRequiredService<IConfiguration>();
        if (!string.IsNullOrEmpty(config.InformationVersion))
        {
            Title = $"{Application.Current!.Name} ({config.InformationVersion})";
        }
        else
        {
            Title = $"{Application.Current!.Name}";
        }

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
