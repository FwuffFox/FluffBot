using System.Text.Json;
using Discord;
using Discord.Commands;
using FluffBot.Extensions;
using Flurl;

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

    private readonly Url _url = "https://www.boredapi.com/api/activity";
        
    [Command("boredom")]
    [Summary("Find an activity to do")]
    private async Task BoredomApi(BoredomNameableArgs? args = null)
    {
        Url? fullUrl = _url.SetQueryParams(new
            {
                type = args?.Type,
                key = args?.Key,
            });
        HttpResponseMessage response = await _boredomApi.GetAsync(fullUrl);
        var obj = await JsonSerializer.DeserializeAsync<BoredomApiResponse>(
            await response.Content.ReadAsStreamAsync(),
            new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        EmbedBuilder? embedBuilder = new EmbedBuilder()
            .WithTitle(obj.Type.ToUpper())
            .WithDescription(obj.Activity)
            .With(!string.IsNullOrEmpty(obj.Link),
                x => x.WithUrl(obj.Link))
            .WithCurrentTimestamp();

        await Context.Channel.SendMessageAsync(embed: embedBuilder.Build());
        if (!string.IsNullOrEmpty(obj.Link)) await Context.Channel.SendMessageAsync($"Link to more info: {obj.Link}");
    }
}

[NamedArgumentType]
public class BoredomNameableArgs
{
    public string? Type { get; set; } = null;
    public string? Key { get; set; } = null;
}

public struct BoredomApiResponse
{
    public string Activity { get; set; }
    public string Type { get; set; }
    
    public string Link { get; set; }
}