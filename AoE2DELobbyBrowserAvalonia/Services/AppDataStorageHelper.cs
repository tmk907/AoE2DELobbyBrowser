using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowserAvalonia.Services
{
    public class AppDataStorageHelper
    {
        private readonly string _localFolder;

        public AppDataStorageHelper(IConfiguration configuration)
        {
            _localFolder = configuration.AppDataFolder;
        }

        public async Task CreateFileAsync(string fileName, object data)
        {
            var path = Path.Combine(_localFolder, fileName);
            await File.WriteAllTextAsync(path, JsonSerializer.Serialize(data));
        }

        public async Task<T> ReadFileAsync<T>(string fileName, Func<T> createDefaultValue)
        {
            try
            {
                var path = Path.Combine(_localFolder, fileName);
                var data = await File.ReadAllTextAsync(path);
                return JsonSerializer.Deserialize<T>(data);
            }
            catch (Exception)
            {
                return createDefaultValue();
            }
        }
    }
}
