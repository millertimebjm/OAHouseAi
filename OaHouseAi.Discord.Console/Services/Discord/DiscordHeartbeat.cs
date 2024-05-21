
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OAHouseChatGpt.Services.Discord;

public class DiscordHeartbeat
{
    [JsonPropertyName("heartbeat_interval")]
    public int? HeartbeatInterval { get; set; }

    public static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions()
        {
            TypeInfoResolver = new DiscordHeartbeatJsonSerializerContext(),
        };
    }

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public string Serialize()
    {
        return JsonSerializer.Serialize(this, GetJsonSerializerOptions());
    }
}

[JsonSerializable(typeof(DiscordHeartbeat))]
public partial class DiscordHeartbeatJsonSerializerContext : JsonSerializerContext
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