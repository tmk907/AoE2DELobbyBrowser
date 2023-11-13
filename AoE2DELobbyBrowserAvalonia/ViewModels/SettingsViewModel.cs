using AoE2DELobbyBrowserAvalonia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowserAvalonia.ViewModels
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
        public SettingsViewModel()
        {
            _newLobbyHighlightTime = AppSettings.NewLobbyHighlightTime.TotalSeconds;
            _separator = AppSettings.Separator;
            _joinLink = AppSettings.JoinLinkType;

            Version = "0.0.1";
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(NewLobbyHighlightTime):
                    AppSettings.NewLobbyHighlightTime = TimeSpan.FromSeconds(_newLobbyHighlightTime);
                    break;
                case nameof(Separator):
                    AppSettings.Separator = _separator;
                    break;
                case nameof(JoinLink):
                    AppSettings.JoinLinkType = _joinLink;
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
            var logsFolder = Ioc.Default.GetRequiredService<IConfiguration>().LogsFolder;
            Directory.CreateDirectory(logsFolder);
            var launcher = Ioc.Default.GetRequiredService<ILauncherService>();
            await launcher.OpenFolderAsync(logsFolder);
        }

        [RelayCommand]
        private async Task RateApp()
        {
            var productId = "9NTQFS6RCXL8";
            var launcher = Ioc.Default.GetRequiredService<ILauncherService>();
            await launcher.LauchUriAsync(new Uri($"ms-windows-store://review/?ProductId={productId}"));
        }

        [RelayCommand]
        private async Task OpenUrl(string url)
        {
            var launcher = Ioc.Default.GetRequiredService<ILauncherService>();
            await launcher.LauchUriAsync(new Uri(url));
        }
    }
}
