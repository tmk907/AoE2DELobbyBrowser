using AoE2DELobbyBrowserAvalonia.Services;
using Config.Net;

namespace AoE2DELobbyBrowserAvalonia
{
    public interface IAppSettings
    {
        int? NewLobbyHighlightTime { get; set; }

        [Option(DefaultValue = JoinLink.Aoe2de)]
        JoinLink JoinLinkType { get; set; }

        string? Separator { get; set; }

        string? LobbySettings { get; set; }
    }
}
