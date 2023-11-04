using AoE2DELobbyBrowserAvalonia.Services;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using FluentAvalonia.UI.Controls;
using System;
using System.Diagnostics;

namespace AoE2DELobbyBrowserAvalonia;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
    }

    public bool IsAoe2deLink => AppSettings.JoinLinkType == JoinLink.Aoe2de;
    public bool IsSteamLink => AppSettings.JoinLinkType == JoinLink.Steam;

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
        AppSettings.JoinLinkType = JoinLink.Aoe2de;
    }

    private void Steam_RadioButton_Checked(object sender, RoutedEventArgs e)
    {
        AppSettings.JoinLinkType = JoinLink.Steam;
    }

    private async void OpenLogsFolderClicked(object sender, RoutedEventArgs e)
    {
        var logsFolder = Ioc.Default.GetRequiredService<IPlatformConfiguration>().LogsFolder;
        var launcher = Ioc.Default.GetRequiredService<ILauncherService>();
        await launcher.OpenFolderAsync(logsFolder);
    }

    private async void RateAppClicked(object sender, RoutedEventArgs e)
    {
        var productId = "9NTQFS6RCXL8";
        Process.Start("open", $"ms-windows-store://review/?ProductId={productId}");
        var launcher = Ioc.Default.GetRequiredService<ILauncherService>();
        await launcher.LauchUriAsync(new Uri($"ms-windows-store://review/?ProductId={productId}"));
    }
}