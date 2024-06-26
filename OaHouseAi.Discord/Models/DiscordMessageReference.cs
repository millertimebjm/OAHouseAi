using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using OaHouseAi.Discord.Services;

namespace OaHouseAi.Discord.Models;

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
            TypeInfoResolver = new DiscordJsonSerializerContext(),
        };
    }

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public string Serialize()
    {
        return JsonSerializer.Serialize(this, GetJsonSerializerOptions());
    }
}
