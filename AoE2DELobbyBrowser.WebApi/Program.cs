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

app.MapGet("/api/lobbies", async (IHttpClientFactory httpClientFactory, ApiCache cache) =>
{
    var data = await cache.GetOrCreateAsync("cachedaoe2netLobbies", () =>
    {
        try
        {
            var url = "https://aoe2.net/api/lobbies?game=aoe2de";
            var httpClient = httpClientFactory.CreateClient();
            return httpClient.GetStringAsync(url);
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
            return Task.FromResult("");
        }
    });

    return Results.Content(data, contentType: MediaTypeNames.Application.Json);
});

app.MapGet("/api/v1/lobbies", async (IHttpClientFactory httpClientFactory, ApiCache cache) =>
{
    var data = await cache.GetOrCreateAsync("cachedLobbies", async () =>
    {
        try
        {
            var url = "https://aoe2.net/api/lobbies?game=aoe2de";
            var httpClient = httpClientFactory.CreateClient();
            var result = await httpClient.GetFromJsonAsync<IEnumerable<AoE2DELobbyBrowser.WebApi.Aoe2net.LobbyDto>>(url);
            if (result != null)
            {
                var lobbies = result.Select(x => LobbyDto.Create(x));
                var serialized = JsonSerializer.Serialize(lobbies);
                return serialized;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
        }
        return "";
    });

    return Results.Content(data, contentType: MediaTypeNames.Application.Json);
});

app.Logger.LogInformation("The application started");
app.Run();