using System.Text.Json.Serialization;

namespace OAHouseChatGpt.Services.ChatGpt;

public class ChatGptResponseModel
{
    //[JsonPropertyName("id")]
    public string Id { get; set; }
    //[JsonPropertyName("object")]
    public string Object { get; set; }
    //[JsonPropertyName("created")]
    public long Created { get; set; }
    //[JsonPropertyName("model")]
    public string Model { get; set; }
    //[JsonPropertyName("choices")]
    public IEnumerable<Choice> Choices { get; set; }
    //[JsonPropertyName("usage")]
    public Usage Usage { get; set; }
    public CompletionStatusEnum CompletionStatus
    {
        get
        {
            if (!Choices.Any())
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
}

public class Choice
{
    //[JsonPropertyName("index")]
    public int Index { get; set; }
    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; set; }
    //[JsonPropertyName("message")]
    public Message Message { get; set; }
}

public class Message
{
    //[JsonPropertyName("role")]
    public string Role { get; set; }
    //[JsonPropertyName("content")]
    public string Content { get; set; }
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

public enum CompletionStatusEnum
{
    Unknown,
    Failed,
    StoppedEarly,
    Success,
}