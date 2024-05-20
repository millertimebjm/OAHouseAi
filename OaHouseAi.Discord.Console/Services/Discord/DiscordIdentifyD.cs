
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OAHouseChatGpt.Services.Discord;

public class DiscordIdentifyD
{
    [JsonPropertyName("token")]
    public string Token { get; set; }
    [JsonPropertyName("intents")]
    public string Intents { get; set; }
    [JsonPropertyName("properties")]
    public DiscordIdentifyDProperties Properties { get; set; }
    [JsonPropertyName("id")]
    public string MessageId { get; set; }
    [JsonPropertyName("content")]
    public string Content { get; set; }
    [JsonPropertyName("channel_id")]
    public string ChannelId { get; set; }
    [JsonPropertyName("heartbeat_interval")]
    public int? HeartbeatInterval { get; set; }
    [JsonPropertyName("mentions")]
    public List<DiscordUser> Mentions { get; set; }


    public static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions()
        {
            TypeInfoResolver = new DiscordIdentifyDJsonSerializerContext(),
        };
    }

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public string Serialize()
    {
        return JsonSerializer.Serialize(this, GetJsonSerializerOptions());
    }
}

[JsonSerializable(typeof(DiscordIdentifyD))]
public partial class DiscordIdentifyDJsonSerializerContext : JsonSerializerContext
{

}

// var identifyPayload = new
//             {
//                 op = 2,
//                 d = new
//                 {
//                     token = _discordToken,
//                     intents = 513,
//                     properties = new
//                     {
//                         os = "linux",
//                         browser = "disco",
//                         device = "disco"
//                     }
//                 }
//             };