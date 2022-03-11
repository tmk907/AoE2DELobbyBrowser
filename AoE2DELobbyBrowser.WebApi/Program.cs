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
    var data = await cache.GetOrCreateAsync(() =>
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

app.Logger.LogInformation("The application started");
app.Run();