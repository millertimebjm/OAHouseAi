
using System.Text.Json.Serialization;

namespace OAHouseChatGpt.Models.Usages;

public class UsageModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ModelName { get; set; }
    public string Username { get; set; }
    public int TotalTokens { get; set; }
    public DateTime UtcTimestamp { get; set; }
}

[JsonSerializable(typeof(UsageModel))]
public partial class UsageModelJsonSerializerContext : JsonSerializerContext
{

}