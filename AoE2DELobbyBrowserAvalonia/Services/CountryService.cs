using System.Collections.Generic;
using System.IO;

namespace AoE2DELobbyBrowser.Services
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

        private Dictionary<string,string> LoadData()
        {
            //string fullPath = Windows.ApplicationModel.Package.Current.InstalledPath + "/Assets/countries.csv";
            //var lines = File.ReadAllLines(fullPath);
            var countryCodes = new Dictionary<string, string>();
            //foreach(var line in lines )
            //{
            //    var i = line.LastIndexOf(',');
            //    var country = line.Substring(0, i);
            //    var code = line.Substring(i + 1);
            //    countryCodes.Add(code, country);
            //}
            return countryCodes;
        }
    }
}
