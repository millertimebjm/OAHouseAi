using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization.Attributes;

namespace OaHouseAi.Repository.Usages.Models;

public class UsageModel
{
    [BsonElement("_id")]
    [JsonPropertyName("_id")]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null;
    public string ModelName { get; set; }
    public string Username { get; set; }
    public int TotalTokens { get; set; }
    public DateTime UtcTimestamp { get; set; } = DateTime.UtcNow;

    public static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions()
        {
            TypeInfoResolver = new UsageModelJsonSerializerContext(),
        };
    }

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public string Serialize()
    {
        return JsonSerializer.Serialize(this, GetJsonSerializerOptions());
    }
}

[JsonSerializable(typeof(UsageModel))]
public partial class UsageModelJsonSerializerContext : JsonSerializerContext
{
}