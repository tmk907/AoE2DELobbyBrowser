namespace AoE2DELobbyBrowser.WebApi.Reliclink
{
    public class BackgroundApiClient : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<BackgroundApiClient> _logger;
        private readonly TimeSpan _refreshInterval;

        public BackgroundApiClient(IServiceProvider services, IConfiguration configuration, ILogger<BackgroundApiClient> logger)
        {
            _services = services;
            _refreshInterval = TimeSpan.FromSeconds(configuration.GetValue<int>("RefreshInterval"));
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BackgroundApiClient running.");

            using PeriodicTimer timer = new PeriodicTimer(_refreshInterval);
            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    using (var scope = _services.CreateScope())
                    {
                        var lobbiesRepository = scope.ServiceProvider.GetRequiredService<LobbiesRepository>();

                        await lobbiesRepository.RefreshCacheAsync();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("BackgroundApiClient is stopping.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BackgroundApiClient stopped working due to exception");
            }
        }
    }
}
