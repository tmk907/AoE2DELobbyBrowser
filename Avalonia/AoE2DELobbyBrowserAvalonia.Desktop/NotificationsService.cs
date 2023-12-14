using AoE2DELobbyBrowser.Core.Models;
using AoE2DELobbyBrowser.Core.Services;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Microsoft.Toolkit.Uwp.Notifications;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowserAvalonia.Desktop
{
    internal class NotificationsService : INotificationsService
    {
        private readonly WindowsNotificationManager _notificationManager;
        private readonly ILauncherService _launcherService;

        public NotificationsService(ILauncherService launcherService)
        {
            _notificationManager = Program.NotificationManager;
            _launcherService = launcherService;

            var disposable = _notificationManager.Notifications
                .Select(args => Observable.FromAsync(_ => OnNotificationActivated(args)))
                .Concat()
                .Subscribe();
        }

        private async Task OnNotificationActivated(ToastArguments args)
        {
            try
            {
                if (args.TryGetValue("JoinLink", out string link) && !string.IsNullOrEmpty(link))
                {
                    Log.Information($"Notification JoinLink {0}", link);

                    await _launcherService.LauchUriAsync(new Uri(link));
                }
                else if (args.TryGetValue("type", out string type) && type == "lobby notification")
                {
                    if (App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime &&
                        desktopLifetime.MainWindow != null)
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            if (desktopLifetime.MainWindow.WindowState == Avalonia.Controls.WindowState.Minimized)
                            {
                                desktopLifetime.MainWindow.WindowState = Avalonia.Controls.WindowState.Normal;
                            }
                            desktopLifetime.MainWindow.Show();
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
            }
        }

        public void ShowNotifications(IEnumerable<LobbyVM> lobbies)
        {
            Log.Debug($"Show notifications");
            foreach (var lobby in lobbies)
            {
                Log.Debug($"Show notification for {lobby.Name} {lobby.AddedAt} {lobby.MatchId}");
            }

            var group = lobbies.Count() > 3;
            if (group)
            {
                ShowNotificationForGroup(lobbies);
            }
            else
            {
                foreach (var lobby in lobbies)
                {
                    ShowNotification(lobby);
                }
            }
        }

        public void ShowNotificationForGroup(IEnumerable<LobbyVM> lobbies)
        {
            try
            {
                var xmlDoc = new ToastContentBuilder()
                    .AddArgument("type", "lobby notification")
                    .AddText($"{lobbies.Count()} new lobbies")
                    .GetXml();

                _notificationManager.ShowNotification(xmlDoc, DateTimeOffset.Now.AddHours(1));
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        private void ShowNotification(LobbyVM lobby)
        {
            try
            {
                var toastArguments = new ToastArguments
                {
                    { "JoinLink", lobby.JoinLink }
                };
                var xmlDoc = new ToastContentBuilder()
                    .AddArgument("type", "lobby notification")
                    .AddHeader("singlelobby", "New lobby", lobby.MatchId)
                    .AddText(lobby.Name)
                    .AddText(lobby.Players.FirstOrDefault()?.Name ?? "")
                    .AddButton("Join game", ToastActivationType.Foreground, toastArguments.ToString())
                    .GetXml();

                _notificationManager.ShowNotification(xmlDoc, DateTimeOffset.Now.AddHours(1));
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}
