using AoE2DELobbyBrowserAvalonia.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowserAvalonia.Services
{
    public interface INotificationsService
    {
        void ShowNotifications(IEnumerable<LobbyVM> lobbies);
    }
}