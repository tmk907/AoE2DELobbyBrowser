using System.Text.Json.Serialization;

namespace AoE2DELobbyBrowser.WebApi.Aoe2InsightsApi
{
    public class LobbyDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("num_slots")]
        public int NumSlots { get; set; }

        [JsonPropertyName("game_type")]
        public int GameType { get; set; }

        [JsonPropertyName("has_password")]
        public bool HasPassword { get; set; }

        [JsonPropertyName("map_type")]
        public int MapType { get; set; }

        [JsonPropertyName("cheats")]
        public bool Cheats { get; set; }

        [JsonPropertyName("map_size")]
        public int MapSize { get; set; }

        [JsonPropertyName("max_pop")]
        public int MaxPop { get; set; }

        [JsonPropertyName("resources")]
        public int Resources { get; set; }

        [JsonPropertyName("speed")]
        public int Speed { get; set; }

        [JsonPropertyName("server")]
        public string Server { get; set; }

        [JsonPropertyName("slots")]
        public Slots Slots { get; set; }

        public List<PlayerDto> Players
        {
            get
            {
                var players = new List<PlayerDto>();
                if (Slots._1 is not null) players.Add(Slots._1);
                if (Slots._2 is not null) players.Add(Slots._2);
                if (Slots._3 is not null) players.Add(Slots._3);
                if (Slots._4 is not null) players.Add(Slots._4);
                if (Slots._5 is not null) players.Add(Slots._5);
                if (Slots._6 is not null) players.Add(Slots._6);
                if (Slots._7 is not null) players.Add(Slots._7);
                if (Slots._8 is not null) players.Add(Slots._8);
                return players;
            }
        }
    }

    public class Slots
    {
        [JsonPropertyName("1")]
        public PlayerDto _1 { get; set; }

        [JsonPropertyName("2")]
        public PlayerDto _2 { get; set; }

        [JsonPropertyName("3")]
        public PlayerDto _3 { get; set; }

        [JsonPropertyName("4")]
        public PlayerDto _4 { get; set; }

        [JsonPropertyName("5")]
        public PlayerDto _5 { get; set; }

        [JsonPropertyName("6")]
        public PlayerDto _6 { get; set; }

        [JsonPropertyName("7")]
        public PlayerDto _7 { get; set; }

        [JsonPropertyName("8")]
        public PlayerDto _8 { get; set; }
    }
}
