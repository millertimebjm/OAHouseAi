using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OAHouseChatGpt.Services.ChatGpt;

public class ChatGptBodyModel
{    
    [JsonPropertyName("model")]
    public string Model { get; set; }
    [JsonPropertyName("messages")]
    public IEnumerable<ChatGptMessageModel> Messages { get; set; }

    public static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions()
        {
            TypeInfoResolver = new ChatGptBodyModelJsonSerializerContext(),
        };
    }

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public string Serialize()
    {
        return JsonSerializer.Serialize(this, GetJsonSerializerOptions());
    }
}

[JsonSerializable(typeof(ChatGptBodyModel))]
public partial class ChatGptBodyModelJsonSerializerContext : JsonSerializerContext
{

}