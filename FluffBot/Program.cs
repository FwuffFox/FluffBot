using System.Reflection;
using System.Text.Json;
using Discord;
using Discord.Addons.Hosting;
using Discord.Interactions;
using Discord.WebSocket;
using FluffBot.Extensions;
using FluffBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using JsonSerializer = FluffBot.Services.JsonSerializer;
using RunMode = Discord.Commands.RunMode;

namespace FluffBot;

public static class Program
{
    public static async Task Main(string[] args)
    {
        IHostBuilder builder = new HostBuilder()
            .ConfigureLogging(x =>
            {
                x.AddConsole();
                x.SetMinimumLevel(LogLevel.Debug);
            })
            .ConfigureAppConfiguration(x =>
            {
                IConfigurationRoot config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("secret.config.json", true, true)
                    .AddUserSecrets(Assembly.GetEntryAssembly()!, true, true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args)
                    .Build();
                
                x.AddConfiguration(config);
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
            .UseInteractionService((context, config) =>
            {
                config.LogLevel = LogSeverity.Debug;
            })
            .ConfigureServices((context, services) =>
            {
                services
                    .AddHostedService<CommandHandler>()
                    .AddHostedService<InteractionHandler>()
                    .AddSingleton(new JsonSerializer(new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    }));
            })
            .UseConsoleLifetime();

        
        using IHost host = builder.Build();
        await host.RunAsync();
    }
}