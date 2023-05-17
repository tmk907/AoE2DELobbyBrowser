using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace AoE2DELobbyBrowser.WebApi.Reliclink
{
    public class OptionsDecoder
    {
        public const int GameTypeKey = 5;
        public const int MapTypeKey = 10;
        public const int GameSpeedKey = 41;

        private static byte[] DecodeZLib(byte[] input)
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
            var separators = new[] { "\u0003\0\0\0", "\u0004\0\0\0", "\u0005\0\0\0", "\u0006\0\0\0", "\b\0\0\0", "\f\0\0\0", "\u001f\0\0\0", "\t\0\0\0" };
            var splitted = decoded.Split(separators, StringSplitOptions.None);
            return splitted.Where(x => x.Contains(':'))
                .ToDictionary(x => int.Parse(x.Split(':')[0]), x => x.Split(':')[1]);
        }
    }
}
