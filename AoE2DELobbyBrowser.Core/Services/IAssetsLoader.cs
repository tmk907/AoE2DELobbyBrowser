using System.IO;
using System;

namespace AoE2DELobbyBrowser.Core.Services
{
    public interface IAssetsLoader
    {
        Stream Open(Uri uri);
        Stream OpenCountries();
    }
}
