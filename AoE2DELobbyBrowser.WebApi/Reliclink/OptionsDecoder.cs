using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace AoE2DELobbyBrowser.WebApi.Reliclink
{
    public class OptionsDecoder
    {
        public const int GameTypeKey = 6;
        public const int MapTypeKey = 11;
        public const int GameSpeedKey = 42;

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
            var separators = new[] { "\u0003\0\0\0", "\u0004\0\0\0", "\u0005\0\0\0", "\u0006\0\0\0" };
            var splitted = decoded.Split(separators, StringSplitOptions.None);
            return splitted.Where(x => x.Contains(':'))
                .ToDictionary(x => int.Parse(x.Split(':')[0]), x => x.Split(':')[1]);
        }
    }
}
