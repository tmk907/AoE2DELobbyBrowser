using System.Text.Json.Serialization;

namespace AoE2DELobbyBrowser.WebApi.Reliclink
{
    public class Match
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("steamlobbyid")]
        public object Steamlobbyid { get; set; }

        [JsonPropertyName("xboxsessionid")]
        public string Xboxsessionid { get; set; }

        [JsonPropertyName("host_profile_id")]
        public int HostProfileId { get; set; }

        [JsonPropertyName("state")]
        public int State { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("visible")]
        public int Visible { get; set; }

        [JsonPropertyName("mapname")]
        public string Mapname { get; set; }

        [JsonPropertyName("options")]
        public string Options { get; set; }

        [JsonPropertyName("passwordprotected")]
        public int Passwordprotected { get; set; }

        [JsonPropertyName("maxplayers")]
        public int Maxplayers { get; set; }

        [JsonPropertyName("slotinfo")]
        public string Slotinfo { get; set; }

        [JsonPropertyName("matchtype_id")]
        public int MatchtypeId { get; set; }

        [JsonPropertyName("matchmembers")]
        public List<Matchmember> Matchmembers { get; set; }

        [JsonPropertyName("observernum")]
        public int Observernum { get; set; }

        [JsonPropertyName("observermax")]
        public int Observermax { get; set; }

        [JsonPropertyName("isobservable")]
        public int IsObservable { get; set; }

        [JsonPropertyName("observerdelay")]
        public int Observerdelay { get; set; }

        [JsonPropertyName("hasobserverpassword")]
        public int Hasobserverpassword { get; set; }

        [JsonPropertyName("servicetype")]
        public int Servicetype { get; set; }

        [JsonPropertyName("relayserver_region")]
        public string RelayserverRegion { get; set; }
    }
}
