using System.Reactive.Subjects;
using Discord.Interactions;

namespace FluffBot.Commands.Slash;

public class BasicSlashCommands : InteractionModuleBase
{
    [SlashCommand("echo", "echo an input")]
    public async Task Echo(string input)
    {
        await RespondAsync(input);
    }
}