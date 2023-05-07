using System.Reflection;
using Discord;
using Discord.Addons.Hosting;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FluffBot.Services;

public class InteractionHandler : DiscordClientService
{
    private DiscordSocketClient _client;
    private ILogger<InteractionHandler> _logger;
    private IConfiguration _configuration;
    private IServiceProvider _serviceProvider;
    private InteractionService _interactionService;
    
    public InteractionHandler(DiscordSocketClient client,
        ILogger<InteractionHandler> logger,
        IConfiguration configuration,
        IServiceProvider serviceProvider,
        InteractionService interactionService) : base(client, logger)
    {
        _client = client;
        _logger = logger;
        _configuration = configuration;
        _serviceProvider = serviceProvider;
        _interactionService = interactionService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _client.ButtonExecuted += ClientOnButtonExecuted;
        _interactionService.InteractionExecuted += InteractionServiceOnInteractionExecuted;
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
        
        _logger.LogInformation($"{nameof(InteractionHandler)} is up and Running!");
    }

    private async Task InteractionServiceOnInteractionExecuted(ICommandInfo info,
        IInteractionContext context,
        IResult result)
    {
        if (result.IsSuccess)
        {
            return;
        }

        await context.Channel.SendMessageAsync(result.ErrorReason);
    }


    private async Task ClientOnButtonExecuted(SocketMessageComponent interaction)
    {
        var ctx = new SocketInteractionContext<SocketMessageComponent>(_client, interaction);
        await _interactionService.ExecuteCommandAsync(ctx, _serviceProvider);
    }
}