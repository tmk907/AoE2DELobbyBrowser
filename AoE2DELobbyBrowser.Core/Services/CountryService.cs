using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;

namespace AoE2DELobbyBrowser.Core.Services
{
    public class CountryService
    {
        private Dictionary<string, string> _countryCodes;

        public string GetCountryName(string code)
        {
            if (_countryCodes == null)
            {
                _countryCodes = LoadData();
            }

            if (code != null && _countryCodes.TryGetValue(code, out var countryName))
            {
                return countryName;
            }
            return "";
        }

        private Dictionary<string, string> LoadData()
        {
            List<string> lines = new List<string>();
            var loader = Ioc.Default.GetRequiredService<IAssetsLoader>();

            using var stream = loader.Open(new Uri("avares://AoE2DELobbyBrowser.Core/Assets/countries.csv"));
            using var streamReader = new StreamReader(stream);
            while (!streamReader.EndOfStream)
            {
                lines.Add(streamReader.ReadLine());
            }

            var countryCodes = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                var i = line.LastIndexOf(',');
                var country = line.Substring(0, i);
                var code = line.Substring(i + 1);
                countryCodes.Add(code, country);
            }
            return countryCodes;
        }
    }
}
