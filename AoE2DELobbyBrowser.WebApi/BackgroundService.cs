using AoE2DELobbyBrowser.WebApi.Aoe2netWebsocket;

namespace AoE2DELobbyBrowser.WebApi
{
    public class MyBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<MyBackgroundService> _logger;
        private readonly LobbiesRepository _lobbiesRepository;
        private readonly IConfiguration _configuration;

        public MyBackgroundService(IServiceProvider services, ILogger<MyBackgroundService> logger,
            LobbiesRepository lobbiesRepository, IConfiguration configuration)
        {
            _services = services;
            _logger = logger;
            _lobbiesRepository = lobbiesRepository;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background task started");
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    try
                    {
                        _logger.LogInformation("Create new websocketClient");
                        var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
                        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Aoe2WebsocketClient>>();
                        var websocketClient = new Aoe2WebsocketClient(logger, httpClientFactory);

                        await websocketClient.InitializeAsync();
                        websocketClient.LobbiesReceived += WebsocketClient_LobbiesReceived;

                        var connectionDuration = _configuration.GetValue<int>("WebsocketConnectionDuration");
                        await Task.Delay(TimeSpan.FromSeconds(connectionDuration));

                        websocketClient.LobbiesReceived -= WebsocketClient_LobbiesReceived;
                        websocketClient.Dispose();
                        _logger.LogInformation("WebsocketClient disposed");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.ToString());
                        await Task.Delay(1000);
                    }
                }
            }
            _logger.LogInformation("Background task cancelled");
        }

        private void WebsocketClient_LobbiesReceived(object? sender, List<Aoe2netWebsocket.LobbyDto> e)
        {
            _lobbiesRepository.AddLobbies(e);
        }
    }
}
