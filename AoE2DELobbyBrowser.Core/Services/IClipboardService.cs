using System.Threading.Tasks;

namespace AoE2DELobbyBrowser.Core.Services
{
    public interface IClipboardService
    {
        Task SetTextAsync(string text);
    }
}
