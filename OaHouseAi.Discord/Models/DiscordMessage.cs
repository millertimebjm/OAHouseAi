using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OaHouseAi.Discord.Models;

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
    [JsonPropertyName("mentions")]
    public List<DiscordUser> MentionedUsers { get; set; }

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

// "mentions":[{"username":"OAHouseChatGpt-Test","public_flags":0,"member":{"roles":["1082692723452162071"],"premium_since":null,"pending":false,"nick":null,"mute":false,"joined_at":"2023-03-07T15:54:20.813000+00:00","flags":0,"deaf":false,"communication_disabled_until":null,"avatar":null},"id":"1082692016426713148","global_name":null,"discriminator":"4773","clan":null,"bot":true,"avatar_decoration_data":null,"avatar":null}]