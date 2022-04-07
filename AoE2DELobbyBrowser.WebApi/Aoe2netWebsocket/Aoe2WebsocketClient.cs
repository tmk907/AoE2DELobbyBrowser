using System.Net.WebSockets;
using System.Text.Json;
using Websocket.Client;

namespace AoE2DELobbyBrowser.WebApi.Aoe2netWebsocket
{
    public class Aoe2WebsocketClient : IDisposable
    {
        private IWebsocketClient client;
        private bool isInitialized = false;
        private readonly ILogger<Aoe2WebsocketClient> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public event EventHandler<List<LobbyDto>> LobbiesReceived;

        public Aoe2WebsocketClient(ILogger<Aoe2WebsocketClient> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task InitializeAsync()
        {
            if (isInitialized) return;
            isInitialized = true;

            var httpClient = _httpClientFactory.CreateClient();
            string cookieValue = await GetCookieAsync(httpClient);

            var factory = new Func<ClientWebSocket>(() =>
            {
                var client = new ClientWebSocket
                {
                    Options =
                    {
                        KeepAliveInterval = TimeSpan.FromSeconds(5),
                    }
                };
                client.Options.SetRequestHeader("Cookie", cookieValue);
                client.Options.SetRequestHeader("Origin", "aoe2.net");
                client.Options.SetRequestHeader("Origin", "https://aoe2.net");
                return client;
            });

            var url = new Uri("wss://aoe2.net/ws");
            Websocket.Client.Logging.LogProvider.IsDisabled = true;

            client = new WebsocketClient(url, factory);
            client.Name = "aoe2.net";
            client.ReconnectTimeout = TimeSpan.FromSeconds(30);
            client.ErrorReconnectTimeout = TimeSpan.FromSeconds(30);
            client.ReconnectionHappened.Subscribe(info =>
            {
                _logger.LogInformation($"Reconnection happened, type: {info.Type}, url: {client.Url}");
            });
            client.DisconnectionHappened.Subscribe(info =>
                _logger.LogWarning($"Disconnection happened, type: {info.Type}"));

            //var receivedMessages = client.MessageReceived
            //    .Select(x => (Response: x, Msg: JsonSerializer.Deserialize<WebsocketMessage<object>>(x.Text)));

            //receivedMessages
            //    .Where(x => x.Msg.Message == "ping")
            //    .Select(x => x.Msg)
            //    .Do(msg => _logger.LogInformation("Ping message received {0}", msg))
            //    .ObserveOn(TaskPoolScheduler.Default)
            //    .Subscribe(_ => SendPing());

            //receivedMessages
            //    .Where(msg => msg.Msg.Message == "lobbies")
            //    .Do(msg => _logger.LogInformation("Lobbies message received"))
            //    .ObserveOn(TaskPoolScheduler.Default)
            //    .Select(msg => JsonSerializer.Deserialize<LobbiesWebsocketMessage>(msg.Response.Text))
            //    .Subscribe();


            client.MessageReceived.Subscribe(msg =>
            {
                var message = JsonSerializer.Deserialize<WebsocketMessage<object>>(msg.Text);
                if (message.Message == "ping")
                {
                    _logger.LogInformation($"Ping message received {msg.Text}");
                    SendPing();
                }
                else if (message.Message == "lobbies")
                {
                    _logger.LogInformation($"Lobbies message received");
                    var lobbiesMessage = JsonSerializer.Deserialize<LobbiesWebsocketMessage>(msg.Text);
                    LobbiesReceived?.Invoke(this, lobbiesMessage.Data);
                }
                else
                {
                    _logger.LogWarning($"Unknown message received {msg.Text}");
                }
            });
            _logger.LogInformation("Initialized");
            await client.Start();
            _logger.LogInformation("Started");
            SendSubscribe();
        }
        private async Task<string> GetCookieAsync(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Add("Host", "aoe2.net");
            var response = await httpClient.GetAsync("https://aoe2.net");
            response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string>? values);
            string cookieValue = values?.FirstOrDefault() ?? "";
            return cookieValue;
        }

        private void SendPing()
        {
            var time = DateTimeOffset.Now.ToUnixTimeSeconds();
            var message = new PingWebsocketMessage
            {
                Data = time,
                Message = "ping"
            };
            SendMessage(message);
        }

        private void SendSubscribe()
        {
            var message = new SubscribeWebsocketMessage
            {
                Message = "subscribe",
                Subscribe = new List<int> { 813780 }
            };
            SendMessage(message);
        }

        private void SendMessage(object message)
        {
            if (!client.IsRunning)
            {
                _logger.LogWarning("Cant send message");
                return;
            }

            var serialized = JsonSerializer.Serialize(message);
            client.Send(serialized);
            _logger.LogInformation($"Sent message {serialized}");
        }

        public void Dispose()
        {
            _logger.LogInformation("Dispose Aoe2WebsocketClient");
            client?.Dispose();
        }
    }
}
