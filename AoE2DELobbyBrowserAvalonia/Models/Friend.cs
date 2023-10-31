using AoE2DELobbyBrowser.Api;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AoE2DELobbyBrowser.Models
{
    public class Friend : ObservableObject
    {
        public Player Player { get; set; }

        public bool IsOnline { get; private set; }

        public Lobby Lobby { get; private set; }

        public Friend(string name, string profileId, string country)
        {
            Player = new Player(name, profileId, country);
        }

        public void UpdateLobby(Lobby lobby)
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

        public static Friend Create(SteamPlayerDto dto)
        {
            return new Friend(dto.PersonaName, dto.SteamId, dto.LocCountryCode);
        }
    }
}
