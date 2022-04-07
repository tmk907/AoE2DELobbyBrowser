using AoE2DELobbyBrowser.WebApi;
using Serilog;
using System.Net.Mime;
using System.Text.Json;

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
builder.Services.AddSingleton<LobbiesRepository>();

builder.Services.AddHostedService<MyBackgroundService>();

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

app.MapGet("/api/lobbies", async (IHttpClientFactory httpClientFactory, ApiCache cache, IConfiguration config) =>
{
    var data = await cache.GetOrCreateAsync("cachedaoe2netLobbies", async () =>
    {
        try
        {
            var url = "https://aoe2.net/api/lobbies?game=aoe2de";
            var httpClient = httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(config.GetValue<int>("TimeoutSeconds"));
            return await httpClient.GetStringAsync(url);
        }
        catch (TaskCanceledException ex)
        {
            Log.Error(ex.ToString());
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
        }
        return "";
    });

    return Results.Content(data, contentType: MediaTypeNames.Application.Json);
});

app.MapGet("/api/v1/lobbies", async (IHttpClientFactory httpClientFactory, ApiCache cache, IConfiguration config) =>
{
    var data = await cache.GetOrCreateAsync("cachedLobbies", async () =>
    {
        try
        {
            var url = "https://aoe2.net/api/lobbies?game=aoe2de";
            var httpClient = httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(config.GetValue<int>("TimeoutSeconds"));
            var result = await httpClient.GetFromJsonAsync<IEnumerable<AoE2DELobbyBrowser.WebApi.Aoe2net.LobbyDto>>(url);
            if (result != null)
            {
                var lobbies = result.Select(x => LobbyDto.Create(x));
                var serialized = JsonSerializer.Serialize(lobbies);
                return serialized;
            }
        }
        catch (TaskCanceledException ex)
        {
            Log.Error(ex.ToString());
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
        }
        return "";
    });

    return Results.Content(data, contentType: MediaTypeNames.Application.Json);
});

app.MapGet("/api/v2/lobbies", (LobbiesRepository lobbiesRepository) =>
{
    var lobbies = lobbiesRepository.GetLobbies();
    return Results.Json(lobbies);
});

app.Logger.LogInformation("The application started");
app.Run();