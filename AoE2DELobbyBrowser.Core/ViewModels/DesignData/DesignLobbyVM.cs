using AoE2DELobbyBrowser.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowser.Core.ViewModels.DesignData
{
    public class DesignLobbyVM : LobbyVM
    {
        public DesignLobbyVM()
        {
            Players.Add(new PlayerVM("Player1", "asd", "PL"));
        }
    }
}
