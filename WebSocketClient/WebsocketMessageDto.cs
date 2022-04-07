﻿using System.Text.Json.Serialization;

namespace WebSocketClient
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

    public class LobbyDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("steamLobbyId")]
        public string SteamLobbyId { get; set; }

        [JsonPropertyName("appId")]
        public int AppId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("numPlayers")]
        public int NumPlayers { get; set; }

        [JsonPropertyName("numSlots")]
        public int NumSlots { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("full")]
        public bool Full { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; }

        [JsonPropertyName("averageRating")]
        public int AverageRating { get; set; }

        [JsonPropertyName("cheats")]
        public bool Cheats { get; set; }

        [JsonPropertyName("fullTechTree")]
        public bool FullTechTree { get; set; }

        [JsonPropertyName("gameType")]
        public string GameType { get; set; }

        [JsonPropertyName("gameTypeId")]
        public int GameTypeId { get; set; }

        [JsonPropertyName("hasPassword")]
        public bool HasPassword { get; set; }

        [JsonPropertyName("hidden")]
        public bool Hidden { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("lockSpeed")]
        public bool LockSpeed { get; set; }

        [JsonPropertyName("lockTeams")]
        public bool LockTeams { get; set; }

        [JsonPropertyName("mapSize")]
        public string MapSize { get; set; }

        [JsonPropertyName("pop")]
        public int Pop { get; set; }

        [JsonPropertyName("ranked")]
        public bool Ranked { get; set; }

        [JsonPropertyName("ratingType")]
        public int RatingType { get; set; }

        [JsonPropertyName("resources")]
        public string Resources { get; set; }

        [JsonPropertyName("server")]
        public string Server { get; set; }

        [JsonPropertyName("sharedExploration")]
        public bool SharedExploration { get; set; }

        [JsonPropertyName("speed")]
        public string Speed { get; set; }

        [JsonPropertyName("startingAge")]
        public string StartingAge { get; set; }

        [JsonPropertyName("turbo")]
        public bool Turbo { get; set; }

        [JsonPropertyName("victory")]
        public string Victory { get; set; }

        [JsonPropertyName("visibility")]
        public string Visibility { get; set; }

        [JsonPropertyName("numSpectators")]
        public int NumSpectators { get; set; }

        [JsonPropertyName("opened")]
        public int Opened { get; set; }

        [JsonPropertyName("players")]
        public List<PlayerDto> Players { get; set; }

        [JsonPropertyName("closed")]
        public int? Closed { get; set; }

        [JsonPropertyName("lastSeen")]
        public int? LastSeen { get; set; }
    }

    public class WebsocketMessage<T>
    {
        [JsonPropertyName("data")]
        public T Data { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    public class LobbiesWebsocketMessage : WebsocketMessage<List<LobbyDto>>
    {
    }

    public class PingWebsocketMessage : WebsocketMessage<long>
    {
    }

    public class SubscribeWebsocketMessage
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("subscribe")]
        public List<int> Subscribe { get; set; }
    }
}
