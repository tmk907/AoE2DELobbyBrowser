using System;
using System.IO;
using System.Net.WebSockets;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Websocket.Client;

namespace WebSocketClient
{
    public class Aoe2WebsocketClient : IDisposable
    {
        private IWebsocketClient client;
        private bool isSubscribed = false;

        public Aoe2WebsocketClient()
        {

        }

        public async Task InitializeAsync()
        {
            var httpClient = new HttpClient();
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
                Log.Information($"Reconnection happened, type: {info.Type}, url: {client.Url}");
            });
            client.DisconnectionHappened.Subscribe(info =>
                Log.Warning($"Disconnection happened, type: {info.Type}"));

            client.MessageReceived.Subscribe(msg =>
            {
                Log.Information($"Message received");
                var message = JsonSerializer.Deserialize<WebsocketMessage<object>>(msg.Text);
                if (message.Message == "ping")
                {
                    Log.Information($"Ping message received {msg.Text}");
                    SendPing();
                }
                else if (message.Message == "lobbies")
                {
                    var lobbiesMessage = JsonSerializer.Deserialize<LobbiesWebsocketMessage>(msg.Text);
                    Log.Information($"{lobbiesMessage.Data.Count()} lobbies");
                    Log.Information(string.Join(", ", lobbiesMessage.Data.Select(x => x.Name)));
                }
            });
            Log.Information("Initialized");
        }

        public async Task Start()
        {
            Log.Information("Start");
            if (!isSubscribed)
            {
                isSubscribed = true;
                await client.Start();
                Log.Information("Started");
                SendSubscribe();
            }
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
                Log.Warning("Cant send message");
                return;
            }

            var serialized = JsonSerializer.Serialize(message);
            client.Send(serialized);
            Log.Information($"Sent message {serialized}");
        }

        public void Dispose()
        {
            client?.Dispose();
        }
    }
}
