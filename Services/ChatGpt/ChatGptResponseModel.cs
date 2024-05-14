using System.Text.Json.Serialization;

namespace OAHouseChatGpt.Services.ChatGpt;

public class ChatGptResponseModel
{
    [JsonPropertyName(nameof(Id))]
    public string Id { get; set; }
    [JsonPropertyName(nameof(Object))]
    public string Object { get; set; }
    [JsonPropertyName(nameof(Created))]
    public long Created { get; set; }
    [JsonPropertyName(nameof(Model))]
    public string Model { get; set; }
    [JsonPropertyName(nameof(Choices))]
    public IEnumerable<Choice> Choices { get; set; }
    [JsonPropertyName(nameof(Usage))]
    public Usage Usage { get; set; }
    [JsonPropertyName(nameof(CompletionStatus))]
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
    [JsonPropertyName(nameof(Index))]
    public int Index { get; set; }
    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; set; }
    [JsonPropertyName(nameof(Message))]
    public Message Message { get; set; }
}

public class Message
{
    [JsonPropertyName(nameof(Role))]
    public string Role { get; set; }
    [JsonPropertyName(nameof(Content))]
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