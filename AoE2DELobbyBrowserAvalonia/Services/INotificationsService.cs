using AoE2DELobbyBrowserAvalonia.Models;
using System.Collections.Generic;

namespace AoE2DELobbyBrowserAvalonia.Services
{
    public interface INotificationsService
    {
        void ShowNotification(LobbyVM lobby);
        void ShowNotificationForGroup(IEnumerable<LobbyVM> lobbies);
        void ShowNotifications(IEnumerable<LobbyVM> lobbies);
    }
}