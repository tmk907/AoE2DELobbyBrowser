using System.Text.Json.Serialization;

namespace AoE2DELobbyBrowser.WebApi.Aoe2netWebsocket
{
    public class PlayerDto
    {
        [JsonPropertyName("steamId")]
        public string SteamId { get; set; }

        [JsonPropertyName("profileId")]
        public int ProfileId { get; set; }

        [JsonPropertyName("slot")]
        public int Slot { get; set; }

        [JsonPropertyName("slotType")]
        public int SlotType { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("clan")]
        public string Clan { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }

        [JsonPropertyName("avatarfull")]
        public string Avatarfull { get; set; }

        [JsonPropertyName("avatarmedium")]
        public string Avatarmedium { get; set; }

        [JsonPropertyName("countryCode")]
        public string CountryCode { get; set; }

        [JsonPropertyName("rating")]
        public int Rating { get; set; }

        [JsonPropertyName("rankedSoloRating")]
        public int RankedSoloRating { get; set; }

        [JsonPropertyName("rankedTeamRating")]
        public int RankedTeamRating { get; set; }

        [JsonPropertyName("unrankedRating")]
        public int UnrankedRating { get; set; }

        [JsonPropertyName("games")]
        public int Games { get; set; }

        [JsonPropertyName("wins")]
        public int Wins { get; set; }

        [JsonPropertyName("streak")]
        public int Streak { get; set; }

        [JsonPropertyName("drops")]
        public int Drops { get; set; }

        [JsonPropertyName("rec")]
        public string Rec { get; set; }

        [JsonPropertyName("color")]
        public int? Color { get; set; }

        [JsonPropertyName("team")]
        public int? Team { get; set; }

        [JsonPropertyName("civ")]
        public int? Civ { get; set; }

        [JsonPropertyName("civAlpha")]
        public int? CivAlpha { get; set; }

        [JsonPropertyName("civName")]
        public string CivName { get; set; }
    }
}
