using System.Text.Json.Serialization;

namespace OAHouseChatGpt.Services.ChatGpt;

public class ChatGptBodyModel
{
    [JsonPropertyName("model")]
    public string Model { get; set; }
    [JsonPropertyName("messages")]
    public IEnumerable<ChatGptMessageModel> Messages { get; set; }
}

[JsonSerializable(typeof(ChatGptBodyModel))]
public partial class ChatGptBodyModelJsonSerializerContext : JsonSerializerContext
{

}