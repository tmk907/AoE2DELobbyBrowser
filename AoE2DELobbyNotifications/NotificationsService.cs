using CommunityToolkit.WinUI.Notifications;
using Serilog;
using System.Collections.Generic;
using System.Linq;

namespace AoE2DELobbyNotifications
{
    internal class NotificationsService
    {
        public NotificationsService()
        {
        }

        public void ShowNotifications(IEnumerable<Lobby> lobbies)
        {
            var group = lobbies.Count() > 3;
            if (group)
            {
                ShowNotificationForGroup(lobbies);
            }
            else
            {
                foreach(var lobby in lobbies)
                {
                    ShowNotification(lobby);
                }
            }
        }

        public void ShowNotificationForGroup(IEnumerable<Lobby> lobbies)
        {
            try
            {
                new ToastContentBuilder()
                    .AddArgument("type", "lobby notification")
                    .AddText($"{lobbies.Count()} new lobbies")
                    .Show();
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        public void ShowNotification(Lobby lobby)
        {
            try
            {
                var toastArguments = new ToastArguments();
                toastArguments.Add("JoinLink", "");
                new ToastContentBuilder()
                    .AddHeader("singlelobby","New lobby",lobby.LobbyId)
                    //.AddText($"New lobby")
                    .AddText(lobby.Name)
                    .AddButton("Join game", ToastActivationType.Foreground, toastArguments.ToString())
                    .Show();
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}
