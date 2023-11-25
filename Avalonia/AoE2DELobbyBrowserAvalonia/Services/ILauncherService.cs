using System;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowserAvalonia.Services;

public interface ILauncherService
{
    Task OpenFileAsync(string filePath);
    Task OpenFolderAsync(string folderPath);
    Task LauchUriAsync(Uri uri);
}
