using System.ComponentModel;

namespace AoE2DELobbyBrowserAvalonia;

public interface IAppConfig
{
    [DefaultValue("")]
    string ApiBaseUrl { get; }
}
