using System;
using System.IO;
using System.Net;
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
    class Main2
    {
        private readonly ManualResetEvent ExitEvent = new ManualResetEvent(false);


        public async Task Start()
        {
            InitLogging();

            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
            AssemblyLoadContext.Default.Unloading += DefaultOnUnloading;
            Console.CancelKeyPress += ConsoleOnCancelKeyPress;

            Console.WriteLine("|=======================|");
            Console.WriteLine("|    WEBSOCKET CLIENT   |");
            Console.WriteLine("|=======================|");
            Console.WriteLine();

            Log.Debug("====================================");
            Log.Debug("              STARTING              ");
            Log.Debug("====================================");

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
            using (IWebsocketClient client = new WebsocketClient(url, factory))
            {
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
                        SendPing(client);
                    }
                    else if (message.Message == "lobbies")
                    {
                        var lobbiesMessage = JsonSerializer.Deserialize<LobbiesWebsocketMessage>(msg.Text);
                        Log.Information($"{lobbiesMessage.Data.Count()} lobbies");
                        Log.Information(string.Join(", ", lobbiesMessage.Data.Select(x => x.Name)));
                    }
                });

                Log.Information("Starting...");
                client.Start().Wait();
                Log.Information("Started.");

                SendSubscribe(client);

                ExitEvent.WaitOne();
            }

            Log.Debug("====================================");
            Log.Debug("              STOPPING              ");
            Log.Debug("====================================");
            Log.CloseAndFlush();
        }

        private async Task<string> GetCookieAsync(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Add("Host", "aoe2.net");
            var response = await httpClient.GetAsync("https://aoe2.net");
            bool hasCookies = response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string>? values);
            string cookieValue = values?.FirstOrDefault() ?? "";
            return cookieValue;
        }

        private void SendPing(IWebsocketClient client)
        {
            var time = DateTimeOffset.Now.ToUnixTimeSeconds();
            var message = new PingWebsocketMessage
            {
                Data = time,
                Message = "ping" 
            };
            SendMessage(client, message);
        }

        private void SendSubscribe(IWebsocketClient client)
        {
            var message = new SubscribeWebsocketMessage 
            {
                Message = "subscribe",
                Subscribe = new List<int> { 813780 }
            };
            SendMessage(client, message);
        }

        private void SendMessage(IWebsocketClient client, object message)
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

        private void InitLogging()
        {
            var executingDir = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            var logPath = Path.Combine(executingDir, "logs", "verbose.log");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                //.WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
                .WriteTo.Console(LogEventLevel.Verbose,
                    theme: AnsiConsoleTheme.Literate,
                    outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message} {NewLine}{Exception}")
                .CreateLogger();
        }

        private void CurrentDomainOnProcessExit(object sender, EventArgs eventArgs)
        {
            Log.Warning("Exiting process");
            ExitEvent.Set();
        }

        private void DefaultOnUnloading(AssemblyLoadContext assemblyLoadContext)
        {
            Log.Warning("Unloading process");
            ExitEvent.Set();
        }

        private void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Log.Warning("Canceling process");
            e.Cancel = true;
            ExitEvent.Set();
        }
    }
}
