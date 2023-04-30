using System.Net;
using System.Text.Json;
using Discord;
using Discord.Commands;

namespace FluffBot;

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

    private readonly HttpClient _boredomApi = new HttpClient();
    
    [Command("boredom")]
    [Summary("Find an activity to do")]
    private async Task BoredomApi()
    {
        var response = await _boredomApi.GetAsync("https://www.boredapi.com/api/activity");
        var obj = await JsonSerializer.DeserializeAsync<BoredomApiResponse>(
            await response.Content.ReadAsStreamAsync(),
            new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            }
            );

        var embedBuilder = new EmbedBuilder()
            .WithTitle(obj.Type)
            .WithDescription(obj.Activity);

        Console.WriteLine(JsonSerializer.Serialize(obj));

        await Context.Channel.SendMessageAsync(embed: embedBuilder.Build());
    }
}

public struct BoredomApiResponse
{
    public string Activity { get; set; }
    public string Type { get; set; }
}