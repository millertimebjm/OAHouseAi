
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OAHouseChatGpt.Services.Discord;

public class DiscordMessageReference
{
    [JsonPropertyName("message_id")]
    public string MessageId { get; set; }
    public bool IsSpecified => !string.IsNullOrWhiteSpace(MessageId);
    [JsonPropertyName("channel_id")]
    public string ChannelId { get; set; }
    [JsonPropertyName("guild_id")]
    public string GuildId { get; set; }
    
    public static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions()
        {
            TypeInfoResolver = new DiscordMessageReferenceJsonSerializerContext(),
        };
    }

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public string Serialize()
    {
        return JsonSerializer.Serialize(this, GetJsonSerializerOptions());
    }
}

[JsonSerializable(typeof(DiscordMessageReference))]
public partial class DiscordMessageReferenceJsonSerializerContext : JsonSerializerContext
{

}