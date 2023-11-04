using System.ComponentModel;

namespace AoE2DELobbyBrowserAvalonia.Desktop;

public interface IAppConfig
{
    [DefaultValue("AvaloniaApplication")]
    string ApplicationName { get; }
}
