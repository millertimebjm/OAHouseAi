using System.Text.Json.Serialization;

namespace OaHouseAi.Discord.Models;

public class DiscordIdentify<T> : DiscordIdentifyBase
{
    [JsonPropertyName("d")]
    public T D { get; set; }
}

[JsonSerializable(typeof(DiscordIdentify<DiscordHeartbeat>))]
[JsonSerializable(typeof(DiscordIdentify<DiscordGatewayIntent>))]
[JsonSerializable(typeof(DiscordIdentify<DiscordMessage>))]
public partial class DiscordIdentifyJsonSerializerContext : JsonSerializerContext
{

}


// [RequiresUnreferencedCode("")]
// [RequiresDynamicCode("")]
// public class DiscordIdentifyConverter<T> : JsonConverter<DiscordIdentify<T>>
// {
//     public override DiscordIdentify<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//     {
//         // Deserialize the JSON manually since the structure of the JSON might vary based on the type of T.
//         // For simplicity, let's assume T can be deserialized directly from the reader.
//         T dValue = JsonSerializer.Deserialize<T>(ref reader, options);

//         // Create a new DiscordIdentify<T> instance with the deserialized value.
//         return new DiscordIdentify<T> { D = dValue };
//     }

//     [RequiresUnreferencedCode("")]
//     [RequiresDynamicCode("")]
//     public override void Write(Utf8JsonWriter writer, DiscordIdentify<T> value, JsonSerializerOptions options)
//     {
//         // Serialize the 'D' property directly.
//         JsonSerializer.Serialize(writer, value.D, options);
//     }
// }

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