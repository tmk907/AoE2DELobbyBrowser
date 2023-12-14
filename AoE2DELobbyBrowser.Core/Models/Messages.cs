using System;

namespace AoE2DELobbyBrowser.Core
{
    public sealed record NavigateToMessage(Type Destination);

    public sealed record NavigateBackMessage();
}
