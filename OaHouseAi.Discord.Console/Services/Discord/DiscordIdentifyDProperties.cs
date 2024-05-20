
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OAHouseChatGpt.Services.Discord;

public class DiscordIdentifyDProperties
{
    [JsonPropertyName("os")]
    public string Os { get; set; }
    [JsonPropertyName("browser")]
    public string Browser { get; set; }
    [JsonPropertyName("device")]
    public string Device { get; set; }
    
    public static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions()
        {
            TypeInfoResolver = new DiscordIdentifyDPropertiesJsonSerializerContext(),
        };
    }

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public string Serialize()
    {
        return JsonSerializer.Serialize(this, GetJsonSerializerOptions());
    }
}

[JsonSerializable(typeof(DiscordIdentifyDProperties))]
public partial class DiscordIdentifyDPropertiesJsonSerializerContext : JsonSerializerContext
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