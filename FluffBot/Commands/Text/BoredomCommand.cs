using Discord;
using Discord.Commands;
using FluffBot.Extensions;
using FluffBot.Services;
using Flurl;

namespace FluffBot.Commands;

public class BoredomCommand : ModuleBase<SocketCommandContext>
{
    private readonly HttpClient _boredomApi = new();
    private readonly JsonSerializer _jsonSerializer;

    private readonly Url _url = "https://www.boredapi.com/api/activity";

    public BoredomCommand(JsonSerializer jsonSerializer)
    {
        _jsonSerializer = jsonSerializer;
    }
        
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
        var obj = await _jsonSerializer.
            DeserializeAsync<BoredomApiResponse>(await response.Content.ReadAsStreamAsync());

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