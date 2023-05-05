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
builder.Services.AddTransient<AoE2DELobbyBrowser.WebApi.v2.Reliclink.LobbiesRepository>();
builder.Services.AddTransient<LobbiesRepository>();


//builder.Services.AddHostedService<MyBackgroundService>();

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

app.MapGet("/api/v3/lobbies", async (LobbiesRepository lobbiesRepository) =>
{
    var lobbies = await lobbiesRepository.GetLobbiesAsync();
    return Results.Json(lobbies);
});

app.MapGet("/api/v3/players", async ([FromQuery] string ids, IConfiguration configuration, IHttpClientFactory httpClientFactory) =>
{
    var url = $"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={configuration["SteamWebApiKey"]}&steamids={ids}";
    var httpClient = httpClientFactory.CreateClient();
    var results = await httpClient.GetFromJsonAsync<SteamPlayerSummaries>(url);
    return Results.Json(results.Response.Players.Select(x=> new SteamPlayerDto
    {
        Avatar = x.Avatar,
        Loccountrycode = x.Loccountrycode,
        Personaname = x.Personaname,
        Profileurl = x.Profileurl,
        Steamid = x.Steamid
    }));
});

app.Logger.LogInformation("The application started");
app.Run();