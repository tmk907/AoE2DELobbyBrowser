using AoE2DELobbyBrowserAvalonia.Models;
using System.Collections.Generic;

namespace AoE2DELobbyBrowserAvalonia.Services
{
    public interface INotificationsService
    {
        void ShowNotification(Lobby lobby);
        void ShowNotificationForGroup(IEnumerable<Lobby> lobbies);
        void ShowNotifications(IEnumerable<Lobby> lobbies);
    }
}