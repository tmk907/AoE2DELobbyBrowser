using System;

namespace AoE2DELobbyBrowserAvalonia
{
    public sealed record KeyboardShortcutMessage(Avalonia.Input.Key Key, Avalonia.Input.KeyModifiers Modifier);
}
