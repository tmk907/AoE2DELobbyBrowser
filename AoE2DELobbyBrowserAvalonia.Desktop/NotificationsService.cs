using AoE2DELobbyBrowserAvalonia.Models;
using AoE2DELobbyBrowserAvalonia.Services;
using DesktopNotifications;
using DesktopNotifications.Windows;
using Microsoft.Toolkit.Uwp.Notifications;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoE2DELobbyBrowserAvalonia.Desktop
{
    internal class NotificationsService : INotificationsService
    {
        private readonly WindowsNotificationManager _notificationManager;
        private readonly ILauncherService _launcherService;

        public NotificationsService(ILauncherService launcherService)
        {
            _notificationManager = new WindowsNotificationManager();

            _notificationManager.NotificationActivated += OnNotificationActivated;
            _launcherService = launcherService;
        }

        private async void OnNotificationActivated(object? sender, NotificationActivatedEventArgs e)
        {
            Log.Information($"Notification activated {e.ActionId}");

            if (e.ActionId.StartsWith("JoinLink"))
            {
                var url = e.ActionId.Replace("JoinLink|", "");
                await _launcherService.LauchUriAsync(new Uri(url));
            }
        }

        public void ShowNotifications(IEnumerable<LobbyVM> lobbies)
        {
            //Log.Debug($"Show notifications");
            //foreach(var lobby in lobbies)
            //{
            //    Log.Debug($"Show notification for {lobby.Name} {lobby.LobbyId} {lobby.MatchId}");
            //}

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
                var toastArguments = new ToastArguments();
                toastArguments.Add("JoinLink", lobby.JoinLink);
                var xmlDoc = new ToastContentBuilder()
                    .AddArgument("type", "lobby notification")
                    .AddHeader("singlelobby", "New lobby", lobby.MatchId)
                    .AddText(lobby.Name)
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
