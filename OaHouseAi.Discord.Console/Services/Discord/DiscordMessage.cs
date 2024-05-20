
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OAHouseChatGpt.Services.Discord;

public class DiscordMessage
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("channel_id")]
    public string ChannelId { get; set; }
    [JsonPropertyName("content")]
    public string Content { get; set; }
    [JsonPropertyName("author")]
    public DiscordUser Author { get; set; }
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
    [JsonPropertyName("message_reference")]
    public DiscordMessageReference Reference { get; set; }
    
    public static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions()
        {
            TypeInfoResolver = new DiscordMessageJsonSerializerContext(),
        };
    }

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public string Serialize()
    {
        return JsonSerializer.Serialize(this, GetJsonSerializerOptions());
    }
}

[JsonSerializable(typeof(DiscordMessage))]
public partial class DiscordMessageJsonSerializerContext : JsonSerializerContext
{

}