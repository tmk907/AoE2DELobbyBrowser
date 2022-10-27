using System.Text.Json.Serialization;

namespace AoE2DELobbyBrowser.WebApi.v2.Reliclink
{
    public class Advertisements
    {
        [JsonPropertyName("result")]
        public Result Result { get; set; }

        [JsonPropertyName("matches")]
        public List<Match> Matches { get; set; }

        [JsonPropertyName("avatars")]
        public List<Avatar> Avatars { get; set; }
    }
}
