using AoE2DELobbyBrowserAvalonia.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoE2DELobbyBrowserAvalonia.Services
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

        public void ShowNotificationForGroup(IEnumerable<LobbyVM> lobbies)
        {
            try
            {
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

        public void ShowNotification(LobbyVM lobby)
        {
            try
            {
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
