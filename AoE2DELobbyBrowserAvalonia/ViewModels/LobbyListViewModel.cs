using AoE2DELobbyBrowser.Models;
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
            Lobby.Create(new AoE2DELobbyBrowser.Api.LobbyDto
            {
                Name = "Lobby1",
                Players = new List<AoE2DELobbyBrowser.Api.PlayerDto>(),
                GameType = GameType.Unknown,
                MapType = MapType.Unknown,
                MatchId = "asd",
                NumSlots = 8,
                Speed = GameSpeed.Unknown,
                SteamLobbyId = "asd"
            }),
            Lobby.Create(new AoE2DELobbyBrowser.Api.LobbyDto
            {
                Name = "Lobby2",
                Players = new List<AoE2DELobbyBrowser.Api.PlayerDto>(),
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
