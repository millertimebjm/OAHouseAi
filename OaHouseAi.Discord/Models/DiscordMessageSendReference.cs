using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OaHouseAi.Discord.Models;

public class DiscordMessageSendReference
{
    [JsonPropertyName("message_id")]
    public string MessageId { get; set; }
}

[JsonSerializable(typeof(DiscordMessageSendReference))]
public partial class DiscordMessageSendReferenceJsonSerializerContext : JsonSerializerContext
{

}