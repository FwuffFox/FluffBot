using System.Reflection;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class CommandHandler : DiscordClientService
{
    private readonly IServiceProvider _provider;
    private readonly CommandService _commandService;
    private readonly IConfiguration _config;
    private readonly DiscordSocketClient _client;

    public CommandHandler(DiscordSocketClient client,
        ILogger<CommandHandler> logger,
        IServiceProvider provider, CommandService commandService,
        IConfiguration config) : base(client, logger)
    {
        _provider = provider;
        _commandService = commandService;
        _config = config;
        _client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Client.MessageReceived += ClientOnMessageReceived;
        _commandService.CommandExecuted += CommandServiceOnCommandExecuted;
        await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        
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
        await _commandService.ExecuteAsync(context, argPos, _provider);
    }
}