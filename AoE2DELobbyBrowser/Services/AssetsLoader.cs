using AoE2DELobbyBrowser.Core.Services;
using System;
using System.IO;

namespace AoE2DELobbyBrowser.Services
{
    public class AssetsLoader : IAssetsLoader
    {
        public Stream Open(Uri uri)
        {
            throw new NotImplementedException();
        }

        public Stream OpenCountries()
        {
            string fullPath = Windows.ApplicationModel.Package.Current.InstalledPath + "/Assets/countries.csv";
            return File.OpenRead(fullPath);
        }
    }
}
