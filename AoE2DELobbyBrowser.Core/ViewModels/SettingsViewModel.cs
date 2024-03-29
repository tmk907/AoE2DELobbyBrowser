﻿using AoE2DELobbyBrowser.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowser.Core.ViewModels
{
    public interface ISettingsViewModel
    {
        IAsyncRelayCommand OpenLogsFolderCommand { get; }
        IAsyncRelayCommand RateAppCommand { get; }
        IRelayCommand NavigateBackCommand { get; }
        public double NewLobbyHighlightTime { get; }
        public JoinLinkEnum JoinLink { get; }
        public string Separator { get; }
        public string Version { get; }
        IAsyncRelayCommand<string> OpenUrlCommand { get; }
    }

    public partial class SettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILauncherService _launcherService;
        private readonly AppSettingsService _settingsService;

        public SettingsViewModel()
        {
            _configuration = Ioc.Default.GetRequiredService<IConfiguration>();
            _launcherService = Ioc.Default.GetRequiredService<ILauncherService>();
            _settingsService = Ioc.Default.GetRequiredService<AppSettingsService>();

            _newLobbyHighlightTime = _settingsService.AppSettings.NewLobbyHighlightTime.TotalSeconds;
            _separator = _settingsService.AppSettings.Separator;
            _joinLink = _settingsService.AppSettings.JoinLinkType;
            Version = GetDisplayVersion();
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(NewLobbyHighlightTime):
                    _settingsService.AppSettings.NewLobbyHighlightTime = TimeSpan.FromSeconds(NewLobbyHighlightTime);
                    break;
                case nameof(Separator):
                    _settingsService.AppSettings.Separator = Separator;
                    break;
                case nameof(JoinLink):
                    _settingsService.AppSettings.JoinLinkType = JoinLink;
                    break;
                default:
                    break;
            }
            base.OnPropertyChanged(e);
        }

        [ObservableProperty]
        private double _newLobbyHighlightTime;

        [ObservableProperty]
        private string _separator;

        [ObservableProperty]
        private JoinLinkEnum _joinLink;

        public string Version { get; }

        [RelayCommand]
        private void NavigateBack()
        {
            WeakReferenceMessenger.Default.Send(new NavigateBackMessage());
        }

        [RelayCommand]
        private async Task OpenLogsFolder()
        {
            var logsFolder = _configuration.LogsFolder;
            Directory.CreateDirectory(logsFolder);
            await _launcherService.OpenFolderAsync(logsFolder);
        }

        [RelayCommand]
        private async Task RateApp()
        {
            var productId = "9NTQFS6RCXL8";
            await _launcherService.LauchUriAsync(new Uri($"ms-windows-store://review/?ProductId={productId}"));
        }

        [RelayCommand]
        private async Task OpenUrl(string url)
        {
            await _launcherService.LauchUriAsync(new Uri(url));
        }

        private string GetDisplayVersion()
        {
            var version = _configuration.AssemblyVersion;
            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }
    }
}
