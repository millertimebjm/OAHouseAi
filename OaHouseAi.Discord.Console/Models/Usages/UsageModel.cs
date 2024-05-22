
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace OAHouseChatGpt.Models.Usages;

public class UsageModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ModelName { get; set; }
    public string Username { get; set; }
    public int TotalTokens { get; set; }
    public DateTime UtcTimestamp { get; set; }

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

public class UsageModelBsonSerializer : SerializerBase<UsageModel>
{
    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public override UsageModel Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var usageModel = JsonSerializer.Deserialize<UsageModel>(
            context.Reader.ReadString(),
            UsageModel.GetJsonSerializerOptions());
        return usageModel;
    }

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, UsageModel value)
    {
        // Convert UsageModel to JSON using System.Text.Json
        string jsonString = JsonSerializer.Serialize(value, UsageModel.GetJsonSerializerOptions());

        // Deserialize JSON string to BSON document
        var bsonDocument = BsonDocument.Parse(jsonString);

        // Write BSON document to BSON serialization context
        BsonSerializer.Serialize(context.Writer, bsonDocument);
    }

    public static void RegisterBsonSerializer()
    {
        BsonSerializer.RegisterSerializer(new UsageModelBsonSerializer());
    }
}

[JsonSerializable(typeof(UsageModel))]
public partial class UsageModelJsonSerializerContext : JsonSerializerContext
{

}

[JsonSerializable(typeof(ExpandoObject))]
public partial class ExpandoObjectJsonSerializerContext : JsonSerializerContext
{

}


