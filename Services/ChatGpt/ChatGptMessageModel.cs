using System.Text.Json.Serialization;

namespace OAHouseChatGpt.Services.ChatGpt;

public class ChatGptMessageModel
{
    [JsonPropertyName("role")]
    public string Role { get; set; }
    [JsonPropertyName("content")]
    public string Content { get; set; }
}

[JsonSerializable(typeof(ChatGptMessageModel))]
public partial class ChatGptMessageModelJsonSerializerContext : JsonSerializerContext
{

}