using AoE2DELobbyBrowserAvalonia.Api;
using AoE2DELobbyBrowserAvalonia.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AoE2DELobbyBrowserAvalonia.ViewModels;

public partial class LobbyListViewModel : ViewModelBase
{
    public LobbyListViewModel()
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
        Lobbies.First().IsNew=true;
    }

    public ReadOnlyObservableCollection<Lobby> Lobbies { get; }
}
