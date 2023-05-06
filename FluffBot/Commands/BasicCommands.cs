using Discord;
using Discord.Commands;

namespace FluffBot.Commands;

public class BasicCommands : ModuleBase<SocketCommandContext>
{
    [Command("ping")]
    [Summary("Answers with pong")]
    private async Task Ping()
    {
        await Context.Channel.SendMessageAsync("pong");
    }

    [Command("time")]
    [Summary("Tells a time")]
    private async Task Time()
    {
        EmbedBuilder builder = new EmbedBuilder();
        for (int i = -7; i < 13; i++)
        {
            var time = TimeOnly.FromDateTime(DateTime.Now.AddHours(i));
            builder.AddField($"UTC{i}", time);
        }
        await Context.Channel.SendMessageAsync(embed: builder.Build());
    }

    [Command("coinflip")]
    [Summary("Flips a coin")]
    private async Task Coin()
    {
        var strings = new [] { "Tails", "Head" };
        await Context.Channel.SendMessageAsync(strings[Random.Shared.Next(0, 2)]);
    }
}