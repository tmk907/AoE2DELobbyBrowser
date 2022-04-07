// See https://aka.ms/new-console-template for more information
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using WebSocketClient;

Console.WriteLine("Hello, World!");


//var main2 = new Main2();
//await main2.Start();


InitLogging();
var client = new Aoe2WebsocketClient();
await client.InitializeAsync();
await client.Start();
await Task.Delay(TimeSpan.FromSeconds(20));
client.Dispose();

void InitLogging()
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .WriteTo.Console(LogEventLevel.Verbose,
            theme: AnsiConsoleTheme.Literate,
            outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message} {NewLine}{Exception}")
        .CreateLogger();
}