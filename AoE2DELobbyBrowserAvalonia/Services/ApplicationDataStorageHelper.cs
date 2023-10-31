using System;
using System.Threading.Tasks;

namespace AoE2DELobbyBrowser.Services
{
    public class ApplicationDataStorageHelper
    {
        //private readonly StorageFolder _localFolder = ApplicationData.Current.LocalFolder;
        private IObjectSerializer _objectSerializer;

        public ApplicationDataStorageHelper(IObjectSerializer objectSerializer)
        {
            _objectSerializer = objectSerializer;
        }

        public async Task CreateFileAsync(string fileName, object data)
        {
            //var file = await _localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            //await FileIO.WriteTextAsync(file, _objectSerializer.Serialize(data));
        }

        public async Task<T> ReadFileAsync<T>(string fileName, T defaultValue)
        {
            try
            {
                //var file = await _localFolder.GetFileAsync(fileName);
                //var data = await FileIO.ReadTextAsync(file);
                //return _objectSerializer.Deserialize<T>(data);
                return defaultValue;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
    }
}
