using AoE2DELobbyBrowser.WebApi.Aoe2netWebsocket;

namespace AoE2DELobbyBrowser.WebApi
{
    public class MyBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<MyBackgroundService> _logger;
        private readonly LobbiesRepository _lobbiesRepository;

        public MyBackgroundService(IServiceProvider services, ILogger<MyBackgroundService> logger, LobbiesRepository lobbiesRepository)
        {
            _services = services;
            _logger = logger;
            _lobbiesRepository = lobbiesRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    _logger.LogInformation("Create new websocketClient");
                    var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Aoe2WebsocketClient>>();
                    var websocketClient = new Aoe2WebsocketClient(logger, httpClientFactory);

                    await websocketClient.InitializeAsync();
                    websocketClient.LobbiesReceived += WebsocketClient_LobbiesReceived;

                    await Task.Delay(TimeSpan.FromHours(1));

                    websocketClient.LobbiesReceived -= WebsocketClient_LobbiesReceived;
                    websocketClient.Dispose();
                    _logger.LogInformation("WebsocketClient disposed");
                }
            }
        }

        private void WebsocketClient_LobbiesReceived(object? sender, List<Aoe2netWebsocket.LobbyDto> e)
        {
            _lobbiesRepository.AddLobbies(e);
        }
    }
}
