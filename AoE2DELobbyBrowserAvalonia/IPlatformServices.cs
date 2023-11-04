using Microsoft.Extensions.DependencyInjection;

namespace AoE2DELobbyBrowserAvalonia;

public interface IPlatformServices
{
    void Register(IServiceCollection services);
}
