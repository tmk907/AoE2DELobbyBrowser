using AoE2DELobbyBrowser.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowser.Core.Services
{
    public interface INotificationsService
    {
        void ShowNotifications(IEnumerable<LobbyVM> lobbies);
    }
}