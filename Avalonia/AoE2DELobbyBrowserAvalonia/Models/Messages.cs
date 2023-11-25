using System;

namespace AoE2DELobbyBrowserAvalonia
{
    public sealed record NavigateToMessage(Type Destination);

    public sealed record NavigateBackMessage();

    public sealed record KeyboardShortcutMessage(Avalonia.Input.Key Key, Avalonia.Input.KeyModifiers Modifier);
}
