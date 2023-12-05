using AoE2DELobbyBrowserAvalonia.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace AoE2DELobbyBrowserAvalonia.Services
{
    public enum JoinLinkEnum
    {
        Aoe2de,
        Steam
    }

    public partial class AppSettings : ObservableObject
    {
        [ObservableProperty]
        TimeSpan _newLobbyHighlightTime;

        [ObservableProperty]
        JoinLinkEnum _joinLinkType;

        [ObservableProperty]
        string _separator;

        [ObservableProperty]
        LobbySettings _lobbySettings;
    }
}
