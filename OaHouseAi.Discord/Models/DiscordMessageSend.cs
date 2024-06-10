using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OaHouseAi.Discord.Models;

public class DiscordMessageSend
{
    [JsonPropertyName("content")]
    public string Content { get; set; }
    [JsonPropertyName("message_reference")]
    public DiscordMessageSendReference ReferenceMessage { get; set; }

    public static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions()
        {
            TypeInfoResolver = new DiscordMessageSendJsonSerializerContext(),
        };
    }

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public string Serialize()
    {
        return JsonSerializer.Serialize(this, GetJsonSerializerOptions());
    }
}

[JsonSerializable(typeof(DiscordMessageSend))]
public partial class DiscordMessageSendJsonSerializerContext : JsonSerializerContext
{

}