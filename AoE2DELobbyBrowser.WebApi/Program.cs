using AoE2DELobbyBrowser.WebApi;
using AoE2DELobbyBrowser.WebApi.Database;
using AoE2DELobbyBrowser.WebApi.Dto;
using AoE2DELobbyBrowser.WebApi.Reliclink;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Net.Mime;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

builder.Host.UseSerilog((context, loggerConfig)
    => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDbContextFactory<MatchesDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
    .EnableSensitiveDataLogging());
builder.Services.AddTransient<DatabaseService>();

builder.Services.AddSingleton<ApiCache>();
builder.Services.AddScoped<LobbiesRepository>();
builder.Services.AddSingleton<CachedLobbyService>();
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

app.MapGet("/api/v3/lobbies", async (ApiCache apiCache) =>
{
    try
    {
        var lobbies = apiCache.Get<IEnumerable<LobbyDto>>(ApiCache.LobbiesKey);
        return Results.Json(lobbies);
    }
    catch (Exception ex)
    {
        Log.Error(ex.ToString());
        return Results.BadRequest();
    }
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

app.MapGet("/api/v3/cachedLobbies", async ([AsParameters]CachedLobbiesQuery cachedLobbiesQuery, 
    CachedLobbyService cachedLobbyService) =>
{
    try
    {
        var matches = await cachedLobbyService.GetMatches(cachedLobbiesQuery);
        return Results.Json(matches);
    }
    catch (Exception ex)
    {
        Log.Error(ex.ToString());
        return Results.BadRequest();
    }
});

app.Logger.LogInformation("The application started");
app.Run();