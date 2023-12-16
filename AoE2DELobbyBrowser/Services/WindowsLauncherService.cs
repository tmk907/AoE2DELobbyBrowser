using AoE2DELobbyBrowser.Core.Services;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;

namespace AoE2DELobbyBrowser.Services
{
    public class WindowsLauncherService : ILauncherService
    {
        public async Task OpenFolderAsync(string folderPath)
        {
            await Launcher.LaunchFolderPathAsync(folderPath);
        }

        public async Task OpenFileAsync(string filePath)
        {
            var file = await StorageFile.GetFileFromPathAsync(filePath);
            await Launcher.LaunchFileAsync(file);
        }

        public async Task LauchUriAsync(Uri uri)
        {
            await Launcher.LaunchUriAsync(uri);
        }
    }
}
