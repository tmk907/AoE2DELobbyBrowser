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

        public Stream OpenCountries()
        {
            return Open(new Uri("avares://AoE2DELobbyBrowserAvalonia/Assets/countries.csv"));
        }
    }
}
