using AoE2DELobbyBrowser.Core.Models;
using AoE2DELobbyBrowser.Core.Services;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AoE2DELobbyBrowser.Services
{
    internal class NotificationsService : INotificationsService
    {
        public NotificationsService()
        {
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

        private void ShowNotificationForGroup(IEnumerable<LobbyVM> lobbies)
        {
            try
            {
                var notification = new AppNotificationBuilder()
                    .AddArgument("type", "lobby notification")
                    .AddText($"{lobbies.Count()} new lobbies")
                    .BuildNotification();

                Show(notification);
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
                var payload = new AppNotificationBuilder()
                    .AddArgument("type", "lobby notification")
                    .AddText(lobby.Name)
                    .AddText(lobby.Players.FirstOrDefault()?.Name ?? "")
                    .AddButton(new AppNotificationButton("Join game")
                        .AddArgument("JoinLink", lobby.JoinLink))
                    .BuildNotification()
                    .Payload;

                // AppNotificationBuilder doesn't support headers, add it to the payload directly
                var notification = new AppNotification(AddHeader(payload, "singlelobby", "New lobby", "type=lobby notification"));

                Show(notification);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        private static void Show(AppNotification notification)
        {
            notification.Expiration = DateTimeOffset.Now.AddHours(1);
            notification.ExpiresOnReboot = true;

            AppNotificationManager.Default.Show(notification);

            if (notification.Id == 0)
            {
                Log.Error("App notification failed to show");
            }
        }

        private static string AddHeader(string payload, string id, string title, string arguments)
        {
            var toast = XDocument.Parse(payload);
            toast.Root!.AddFirst(new XElement("header",
                new XAttribute("id", id),
                new XAttribute("title", title),
                new XAttribute("arguments", arguments)));
            return toast.ToString(SaveOptions.DisableFormatting);
        }
    }
}
