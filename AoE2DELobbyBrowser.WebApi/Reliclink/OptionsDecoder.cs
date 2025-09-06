using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.Text.RegularExpressions;

namespace AoE2DELobbyBrowser.WebApi.Reliclink
{
    public class OptionsDecoder
    {
        public const int GameTypeKey = 5;
        public const int MapTypeKey = 10;
        public const int GameSpeedKey = 41;
        public const int ModId = 59;
        public const int Scenario = 38;
        public const int Dataset = 63;


        public static byte[] DecodeZLib(byte[] input)
        {
            var inputStream = new MemoryStream(input);
            using (var zipInput = new InflaterInputStream(inputStream))
            {
                using (var resultStream = new MemoryStream())
                {
                    zipInput.CopyTo(resultStream);
                    return resultStream.ToArray();
                }
            }
        }

        public static string DecodeOptions(string input)
        {
            byte[] decoded = System.Convert.FromBase64String(input);
            byte[] unzipped = DecodeZLib(decoded);
            var text = System.Text.Encoding.ASCII.GetString(unzipped).Replace("\"", "");
            return System.Text.Encoding.ASCII.GetString(System.Convert.FromBase64String(text));
        }

        public static Dictionary<int, string> DecodedToDict(string decoded)
        {
            var splitted = Regex.Split(decoded, @"[\x00-\x1F]+");
            return splitted.Where(x => x.Contains(':'))
                .ToDictionary(x => int.Parse(x.Split(':')[0]), x => x.Split(':')[1]);
        }
    }
}
