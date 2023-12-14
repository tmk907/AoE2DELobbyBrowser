using AoE2DELobbyBrowser.Core.Services;
using Avalonia.Input.Platform;
using CommunityToolkit.Mvvm.DependencyInjection;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowserAvalonia.Services
{
    public class ClipboardService : IClipboardService
    {
        public async Task SetTextAsync(string text)
        {
            var clipboard = Ioc.Default.GetRequiredService<IClipboard>();
            await clipboard.SetTextAsync(text);
        }
    }
}
