using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using System.Linq;

namespace AoE2DELobbyBrowserAvalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.KeyBindings.Add(new Avalonia.Input.KeyBinding()
        {
            Command = this.LogicalChildren.OfType<MainView>().First().FocusSearchCommand,
            Gesture = new Avalonia.Input.KeyGesture(Avalonia.Input.Key.F, Avalonia.Input.KeyModifiers.Control)
        });
        this.KeyBindings.Add(new Avalonia.Input.KeyBinding()
        {
            Command = this.LogicalChildren.OfType<MainView>().First().FocusExcludeCommand,
            Gesture = new Avalonia.Input.KeyGesture(Avalonia.Input.Key.G, Avalonia.Input.KeyModifiers.Control)
        });
        this.KeyBindings.Add(new Avalonia.Input.KeyBinding()
        {
            Command = this.LogicalChildren.OfType<MainView>().First().FocusGameTypesCommand,
            Gesture = new Avalonia.Input.KeyGesture(Avalonia.Input.Key.Q, Avalonia.Input.KeyModifiers.Control)
        });
    }
}
