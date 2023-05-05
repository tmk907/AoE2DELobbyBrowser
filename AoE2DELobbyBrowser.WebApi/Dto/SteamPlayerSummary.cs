using System.Text.Json.Serialization;

namespace AoE2DELobbyBrowser.WebApi.Dto
{
    public class SteamPlayer
    {
        [JsonPropertyName("steamid")]
        public string Steamid { get; set; }

        [JsonPropertyName("communityvisibilitystate")]
        public int Communityvisibilitystate { get; set; }

        [JsonPropertyName("profilestate")]
        public int Profilestate { get; set; }

        [JsonPropertyName("personaname")]
        public string Personaname { get; set; }

        [JsonPropertyName("profileurl")]
        public string Profileurl { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }

        [JsonPropertyName("avatarmedium")]
        public string Avatarmedium { get; set; }

        [JsonPropertyName("avatarfull")]
        public string Avatarfull { get; set; }

        [JsonPropertyName("avatarhash")]
        public string Avatarhash { get; set; }

        [JsonPropertyName("personastate")]
        public int Personastate { get; set; }

        [JsonPropertyName("primaryclanid")]
        public string Primaryclanid { get; set; }

        [JsonPropertyName("timecreated")]
        public int Timecreated { get; set; }

        [JsonPropertyName("personastateflags")]
        public int Personastateflags { get; set; }

        [JsonPropertyName("loccountrycode")]
        public string Loccountrycode { get; set; }

        [JsonPropertyName("locstatecode")]
        public string Locstatecode { get; set; }

        [JsonPropertyName("loccityid")]
        public int Loccityid { get; set; }

        [JsonPropertyName("realname")]
        public string Realname { get; set; }
    }

    public class SteamResponse
    {
        [JsonPropertyName("players")]
        public List<SteamPlayer> Players { get; set; }
    }

    public class SteamPlayerSummaries
    {
        [JsonPropertyName("response")]
        public SteamResponse Response { get; set; }
    }
}
