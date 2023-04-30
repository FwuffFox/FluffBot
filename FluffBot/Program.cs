using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class Program
{
    public static async Task Main(string[] args)
    {
        IHostBuilder builder = new HostBuilder()
            .ConfigureAppConfiguration(x =>
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("secret.config.json", false, true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args)
                    .Build();
                
                x.AddConfiguration(config);
            })
            .ConfigureLogging(x =>
            {
                x.AddConsole();
                x.SetMinimumLevel(LogLevel.Debug);
            })
            .ConfigureDiscordHost((context, config) =>
            {
                config.SocketConfig = new DiscordSocketConfig()
                {
                    GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent,
                    LogLevel = LogSeverity.Debug,
                    AlwaysDownloadUsers = false,
                    AlwaysDownloadDefaultStickers = false,
                    MessageCacheSize = 200,
                };
                config.Token = context.Configuration["BotToken"]
                               ?? throw new Exception("No token provided in configuration");
            })
            .UseCommandService((context, config) =>
            {
                config.CaseSensitiveCommands = false;
                config.LogLevel = LogSeverity.Debug;
                config.DefaultRunMode = RunMode.Async;
            })
            .ConfigureServices((context, services) =>
            {
                services
                    .AddHostedService<CommandHandler>();
            })
            .UseConsoleLifetime();

        
        using var host = builder.Build();
        await host.RunAsync();
    }
}