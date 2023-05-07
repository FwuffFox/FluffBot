using System.Text.Json;

namespace FluffBot.Services;

public class JsonSerializer
{
    private readonly JsonSerializerOptions _options;
    
    public JsonSerializer(JsonSerializerOptions options)
    {
        _options = options;
    }

    public async Task<T?> DeserializeAsync<T>(Stream utf8,
        JsonSerializerOptions? options = null,
        CancellationToken cancellationToken = default) =>
        await System.Text.Json.JsonSerializer.DeserializeAsync<T>(utf8, options ?? _options, cancellationToken);
}