using AoE2DELobbyBrowser.WebApi.Dto;
using AoE2DELobbyBrowser.WebApi.Reliclink;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Net.Mime;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

builder.Logging.ClearProviders();
var logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Logging.AddSerilog(logger);

builder.Services.AddSingleton<ApiCache>();
builder.Services.AddScoped<AoE2DELobbyBrowser.WebApi.v2.Reliclink.LobbiesRepository>();
builder.Services.AddScoped<LobbiesRepository>();
builder.Services.AddHostedService<BackgroundApiClient>();

var app = builder.Build();

app.MapGet("/", () => 
{
    var html = $"<a href='https://github.com/tmk907/AoE2DELobbyBrowser'>AoE2DELobbyBrowser</a>";
    return Results.Content(html, contentType: MediaTypeNames.Text.Html);
});

app.MapGet("/api", () =>
{
    var html = $"This API is used by <a href='https://github.com/tmk907/AoE2DELobbyBrowser'>AoE2DELobbyBrowser</a>";
    return Results.Content(html, contentType: MediaTypeNames.Text.Html);
});

app.MapGet("/api/v2/lobbies", async (AoE2DELobbyBrowser.WebApi.v2.Reliclink.LobbiesRepository lobbiesRepository) =>
{
    var lobbies = await lobbiesRepository.GetLobbiesAsync();
    return Results.Json(lobbies);
});

app.MapGet("/api/v3/lobbies", async (ApiCache apiCache) =>
{
    var lobbies = apiCache.Get<IEnumerable<LobbyDto>>(ApiCache.LobbiesKey);
    return Results.Json(lobbies);
});

app.MapGet("/api/v3/players", async ([FromQuery] string ids, IConfiguration configuration, IHttpClientFactory httpClientFactory) =>
{
    try
    {
        var url = $"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={configuration["SteamWebApiKey"]}&steamids={ids}";
        var httpClient = httpClientFactory.CreateClient();
        var results = await httpClient.GetFromJsonAsync<SteamPlayerSummaries>(url);
        var players = results.Response.Players.Select(x => new SteamPlayerDto
        {
            Avatar = x.Avatar,
            CountryCode = x.Loccountrycode,
            Name = x.Personaname,
            ProfileUrl = x.Profileurl,
            Steamid = x.Steamid,
            Status = x.Personastate
        });
        return Results.Json(players);
    }
    catch (Exception ex)
    {
        Log.Error(ex.ToString());
        return Results.BadRequest();
    }
});

app.Logger.LogInformation("The application started");
app.Run();