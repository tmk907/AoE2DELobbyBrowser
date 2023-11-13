using System.ComponentModel;

namespace AoE2DELobbyBrowserAvalonia;

public interface IAppConfig
{
    [DefaultValue("AvaloniaApplication")]
    string ApplicationName { get; }

    [DefaultValue("aoe2api.dryforest.net")]
    string ApiBaseUrl { get; }
}
