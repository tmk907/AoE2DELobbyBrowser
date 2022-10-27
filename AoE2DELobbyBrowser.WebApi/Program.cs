using AoE2DELobbyBrowser.WebApi;
using AoE2DELobbyBrowser.WebApi.Reliclink;
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
builder.Services.AddSingleton<AoE2DELobbyBrowser.WebApi.v2.Reliclink.LobbiesRepository>();
builder.Services.AddSingleton<LobbiesRepository>();


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

app.Logger.LogInformation("The application started");
app.Run();