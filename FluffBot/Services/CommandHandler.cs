using System.Reflection;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FluffBot.Services;

public class CommandHandler : DiscordClientService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly CommandService _commandService;
    private readonly IConfiguration _config;
    private readonly DiscordSocketClient _client;
    private ILogger<CommandHandler> _logger;

    public CommandHandler(DiscordSocketClient client,
        ILogger<CommandHandler> logger,
        IServiceProvider serviceProvider,
        CommandService commandService,
        IConfiguration config) : base(client, logger)
    {
        _serviceProvider = serviceProvider;
        _commandService = commandService;
        _config = config;
        _client = client;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Client.MessageReceived += ClientOnMessageReceived;
        _commandService.CommandExecuted += CommandServiceOnCommandExecuted;
        await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
        _logger.LogInformation($"{nameof(InteractionHandler)} is up and Running!");
    }

    private async Task CommandServiceOnCommandExecuted(Optional<CommandInfo> commandInfo,
        ICommandContext commandContext, 
        IResult result)
    {
        if (result.IsSuccess)
        {
            return;
        }
        
        await commandContext.Channel.SendMessageAsync(result.ErrorReason);
    }

    private async Task ClientOnMessageReceived(SocketMessage socketMsg)
    {
        if (socketMsg is not SocketUserMessage message) return;
        if (message.Source != MessageSource.User) return;
        
        var argPos = 0;
        if (!message.HasStringPrefix(_config["prefix"], ref argPos) && !message.HasMentionPrefix(_client.CurrentUser, ref argPos)) return;

        var context = new SocketCommandContext(_client, message);
        await _commandService.ExecuteAsync(context, argPos, _serviceProvider);
    }
}