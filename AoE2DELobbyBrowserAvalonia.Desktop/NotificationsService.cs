using AoE2DELobbyBrowserAvalonia.Models;
using AoE2DELobbyBrowserAvalonia.Services;
using DesktopNotifications;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowserAvalonia.Desktop
{
    internal class NotificationsService : INotificationsService
    {
        private readonly INotificationManager _notificationManager;
        private readonly ILauncherService _launcherService;

        public NotificationsService(ILauncherService launcherService)
        {
            _notificationManager = Program.NotificationManager;
            _notificationManager.NotificationActivated += OnNotificationActivated;
            _launcherService = launcherService;
        }

        private async void OnNotificationActivated(object? sender, NotificationActivatedEventArgs e)
        {
            Log.Information($"Notification activated {e.ActionId}");

            if (e.ActionId.StartsWith("JoinLink"))
            {
                var url = e.ActionId.Replace("JoinLink|", "");
                //await _launcherService.LauchUriAsync(new Uri(url));
            }
        }

        public async Task ShowNotifications(IEnumerable<LobbyVM> lobbies)
        {
            //Log.Debug($"Show notifications");
            //foreach(var lobby in lobbies)
            //{
            //    Log.Debug($"Show notification for {lobby.Name} {lobby.LobbyId} {lobby.MatchId}");
            //}

            var group = lobbies.Count() > 3;
            if (group)
            {
                await ShowNotificationForGroup(lobbies);
            }
            else
            {
                foreach (var lobby in lobbies)
                {
                    await ShowNotification(lobby);
                }
            }
        }

        public async Task ShowNotificationForGroup(IEnumerable<LobbyVM> lobbies)
        {
            try
            {
                var notification = new Notification
                {
                    Title = $"{lobbies.Count()} new lobbies",
                };
                await _notificationManager.ShowNotification(notification, DateTimeOffset.Now.AddHours(1));

                //new ToastContentBuilder()
                //    .AddArgument("type", "lobby notification")
                //    .AddText($"{lobbies.Count()} new lobbies")
                //    .Show(toast =>
                //    {
                //        toast.ExpiresOnReboot = true;
                //        toast.ExpirationTime = DateTime.Now.AddHours(1);
                //    });
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        private async Task ShowNotification(LobbyVM lobby)
        {
            try
            {
                var notification = new Notification
                {
                    Title = "New lobby",
                    Body = lobby.Name,
                    Buttons =
                    {
                        ("Join game", $"JoinLink|{lobby.JoinLink}")
                    }
                };
                await _notificationManager.ShowNotification(notification, DateTimeOffset.Now.AddHours(1));

                //var toastArguments = new ToastArguments();
                //toastArguments.Add("JoinLink", lobby.JoinLink);
                //new ToastContentBuilder()
                //    .AddArgument("type", "lobby notification")
                //    .AddHeader("singlelobby", "New lobby", lobby.MatchId)
                //    .AddText(lobby.Name)
                //    .AddButton("Join game", ToastActivationType.Foreground, toastArguments.ToString())
                //    .Show(toast =>
                //    {
                //        toast.ExpiresOnReboot = true;
                //        toast.ExpirationTime = DateTime.Now.AddHours(1);
                //    });
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}
