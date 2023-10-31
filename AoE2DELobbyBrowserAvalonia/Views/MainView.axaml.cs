using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using System.Windows.Input;

namespace AoE2DELobbyBrowserAvalonia.Views;

public partial class MainView : UserControl
{

    public MainView()
    {
        InitializeComponent();
    }

    [RelayCommand]
    public void FocusSearch()
    {
        this.FindControl<TextBox>("searchTextBox")?.Focus(Avalonia.Input.NavigationMethod.Pointer);
    }

    [RelayCommand]
    private void FocusExclude()
    {
        this.FindControl<TextBox>("excludeTextBox")?.Focus();
    }

    [RelayCommand]
    private void FocusGameTypes()
    {
        var c = this.FindControl<ComboBox>("gameTypesCombobox");
        c?.Focus();
    }

    private void UnFocusOnKeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        if (e.Key == Avalonia.Input.Key.Escape || e.Key == Avalonia.Input.Key.Enter) 
        {
            TopLevel.GetTopLevel(this)?.FocusManager?.ClearFocus();
        }
    }
}
