using System;

namespace AoE2DELobbyBrowserAvalonia
{
    public class NavigateToMessage
    {
        public Type Destination { get; set; }
    }

    public class NavigateBackMessage { }

    public sealed record KeyboardShortcutMessage(Avalonia.Input.Key Key, Avalonia.Input.KeyModifiers Modifier);
}
