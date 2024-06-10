using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OaHouseAi.ChatGpt.Models;

public class ChatGptResponseModel
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("object")]
    public string Object { get; set; }
    [JsonPropertyName("created")]
    public long Created { get; set; }
    [JsonPropertyName("model")]
    public string Model { get; set; }
    [JsonPropertyName("choices")]
    public IEnumerable<Choice> Choices { get; set; }
    [JsonPropertyName("usage")]
    public Usage Usage { get; set; }
    public CompletionStatusEnum CompletionStatus
    {
        get
        {
            if (Choices == null || !Choices.Any())
            {
                return CompletionStatusEnum.Failed;
            }
            switch (Choices.First().FinishReason)
            {
                case null:
                    return CompletionStatusEnum.StoppedEarly;
                case "stop":
                    return CompletionStatusEnum.Success;
                default:
                    return CompletionStatusEnum.Unknown;
            }
        }
    }

    public static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions()
        {
            TypeInfoResolver = new ChatGptResponseModelJsonSerializerContext(),
        };
    }

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public string Serialize()
    {
        return JsonSerializer.Serialize(this, GetJsonSerializerOptions());
    }
}

[JsonSerializable(typeof(ChatGptResponseModel))]
public partial class ChatGptResponseModelJsonSerializerContext : JsonSerializerContext
{

}

public class Choice
{
    [JsonPropertyName("index")]
    public int Index { get; set; }
    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; set; }
    [JsonPropertyName("message")]
    public Message Message { get; set; }
}

[JsonSerializable(typeof(Choice))]
public partial class ChoiceJsonSerializerContext : JsonSerializerContext
{

}

public class Message
{
    [JsonPropertyName("role")]
    public string Role { get; set; }
    [JsonPropertyName("content")]
    public string Content { get; set; }
}

[JsonSerializable(typeof(Message))]
public partial class MessageJsonSerializerContext : JsonSerializerContext
{

}

public class Usage
{
    [JsonPropertyName("prompt_tokens")]
    public int ProptTokens { get; set; }
    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; set; }
    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}

[JsonSerializable(typeof(Usage))]
public partial class UsageJsonSerializerContext : JsonSerializerContext
{

}

public enum CompletionStatusEnum
{
    Unknown,
    Failed,
    StoppedEarly,
    Success,
}

