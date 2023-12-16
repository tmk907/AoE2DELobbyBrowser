using AoE2DELobbyBrowser.Core.Services;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace AoE2DELobbyBrowser.Services
{
    public class ClipboardService : IClipboardService
    {
        public Task SetTextAsync(string text)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText(text);
            Clipboard.SetContent(dataPackage);
            return Task.CompletedTask;
        }
    }
}
