using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using AoE2DELobbyBrowserAvalonia.Services;

namespace AoE2DELobbyBrowserAvalonia.Desktop;

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