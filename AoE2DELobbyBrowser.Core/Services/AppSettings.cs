using AoE2DELobbyBrowser.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace AoE2DELobbyBrowser.Core.Services
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
