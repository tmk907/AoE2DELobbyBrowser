using AoE2DELobbyBrowserAvalonia.Api;
using AoE2DELobbyBrowserAvalonia.Models;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AoE2DELobbyBrowserAvalonia.ViewModels.DesignData;

public class DesignLobbyListViewModel : ILobbyListViewModel
{
    public DesignLobbyListViewModel()
    {
        Lobbies = new ReadOnlyObservableCollection<Lobby>(new ObservableCollection<Lobby>
        {
            Lobby.Create(new LobbyDto
            {
                Name = "Lobby1",
                Players = new List<PlayerDto>(),
                GameType = GameType.Unknown,
                MapType = MapType.Unknown,
                MatchId = "asd",
                NumSlots = 8,
                Speed = GameSpeed.Unknown,
                SteamLobbyId = "asd"
            }),
            Lobby.Create(new LobbyDto
            {
                Name = "Lobby2",
                Players = new List<PlayerDto>(),
                GameType = GameType.Unknown,
                MapType = MapType.Unknown,
                MatchId = "asd",
                NumSlots = 8,
                Speed = GameSpeed.Unknown,
                SteamLobbyId = "asd"
            })
        });
        Lobbies.First().IsNew = true;
        SelectedLobby = Lobbies.First();
    }

    public Lobby? SelectedLobby { get; }

    public ReadOnlyObservableCollection<Lobby> Lobbies { get; }

    public IRelayCommand<Lobby> SelectLobbyCommand => throw new System.NotImplementedException();
}
