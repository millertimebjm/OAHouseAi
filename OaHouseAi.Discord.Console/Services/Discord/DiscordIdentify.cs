
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OAHouseChatGpt.Services.Discord;

public class DiscordIdentify
{
    [JsonPropertyName("op")]
    public int Op { get; set; }
    [JsonPropertyName("d")]
    public DiscordIdentifyD D { get; set; }
    [JsonPropertyName("s")]
    public int? S { get; set; }
    [JsonPropertyName("t")]
    public string T { get; set; }

    public static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions()
        {
            TypeInfoResolver = new DiscordIdentifyJsonSerializerContext(),
        };
    }

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public string Serialize()
    {
        return JsonSerializer.Serialize(this, GetJsonSerializerOptions());
    }
}

[JsonSerializable(typeof(DiscordIdentify))]
public partial class DiscordIdentifyJsonSerializerContext : JsonSerializerContext
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