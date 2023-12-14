using AoE2DELobbyBrowser.Core.Services;
using Avalonia.Platform;
using System;
using System.IO;

namespace AoE2DELobbyBrowserAvalonia.Services
{
    public class AssetsLoader : IAssetsLoader
    {
        public Stream Open(Uri uri)
        {
            return AssetLoader.Open(uri);
        }
    }
}
