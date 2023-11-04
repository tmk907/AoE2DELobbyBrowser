using AoE2DELobbyBrowser;
using AoE2DELobbyBrowserAvalonia.Services;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using FluentAvalonia.UI.Controls;
using System;
using System.Diagnostics;
using System.IO;

namespace AoE2DELobbyBrowserAvalonia;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
    }

    public bool IsAoe2deLink => AppSettings.JoinLinkType == AppSettings.JoinLink.Aoe2de;
    public bool IsSteamLink => AppSettings.JoinLinkType == AppSettings.JoinLink.Steam;

    public string Version
    {
        get
        {
            //var version = Windows.ApplicationModel.Package.Current.Id.Version;
            //return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            return "";
        }
    }

    private void GoBack_Click(object sender, RoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new NavigateBackMessage());
    }

    private void newLobbyNumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        double seconds = args.NewValue;
        if (double.IsNaN(seconds)) return;
        AppSettings.NewLobbyHighlightTime = TimeSpan.FromSeconds(seconds);
    }

    private void Aoe2de_RadioButton_Checked(object sender, RoutedEventArgs e)
    {
        AppSettings.JoinLinkType = AppSettings.JoinLink.Aoe2de;
    }

    private void Steam_RadioButton_Checked(object sender, RoutedEventArgs e)
    {
        AppSettings.JoinLinkType = AppSettings.JoinLink.Steam;
    }

    private void separatorTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        //AppSettings.Separator = separatorTextBox.Text;
    }

    private async void OpenLogsFolderClicked(object sender, RoutedEventArgs e)
    {
        try
        {
            Directory.CreateDirectory(App.LogsFolderPath);
        }
        catch (Exception) { }

        using var proc = new Process { StartInfo = { UseShellExecute = true, FileName = $"explorer", Arguments = App.LogsFolderPath } };
        proc.Start();

        //await Launcher.LaunchFolderPathAsync(App.LogsFolderPath);
    }

    private async void RateAppClicked(object sender, RoutedEventArgs e)
    {
        var productId = "9NTQFS6RCXL8";
        Process.Start("open", $"ms-windows-store://review/?ProductId={productId}");
        //bool result = await Launcher.LaunchUriAsync(new Uri($"ms-windows-store://review/?ProductId={productId}"));
    }
}