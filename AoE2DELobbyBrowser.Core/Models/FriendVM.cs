using AoE2DELobbyBrowser.Core.Api;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AoE2DELobbyBrowser.Core.Models
{
    public class FriendVM : ObservableObject
    {
        public PlayerVM Player { get; set; }

        public bool IsOnline { get; private set; }

        public LobbyVM Lobby { get; private set; }

        public FriendVM(string name, string profileId, string country)
        {
            Player = new PlayerVM(name, profileId, country);
        }

        public void UpdateLobby(LobbyVM lobby)
        {
            Lobby = lobby;
            OnPropertyChanged(nameof(Lobby));
            UpdateStatus(-1);
        }

        public void UpdateStatus(int status)
        {
            switch (status)
            {
                case -1:
                    break;
                case 1:
                case 6:
                    IsOnline = true;
                    break;
                default:
                    IsOnline = false;
                    break;
            }
            if (Lobby != null) IsOnline = true;

            OnPropertyChanged(nameof(IsOnline));
        }

        public static FriendVM Create(SteamPlayerDto dto)
        {
            return new FriendVM(dto.PersonaName, dto.SteamId, dto.LocCountryCode);
        }
    }
}
