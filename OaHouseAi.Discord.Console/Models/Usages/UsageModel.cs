
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

    
}

public static class BsonSerializerRegistrationHelper
{
    public static void RegisterBsonSerializer()
    {
        BsonSerializer.RegisterSerializer(new UsageModelBsonSerializer());
        BsonSerializer.RegisterSerializer(new ExpandoObjectBsonSerializer());
    }
}

[JsonSerializable(typeof(UsageModel))]
public partial class UsageModelJsonSerializerContext : JsonSerializerContext
{

}

public class ExpandoObjectBsonSerializer : SerializerBase<ExpandoObject>
{
    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public override ExpandoObject Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var jsonString = context.Reader.ReadString();
        var expandoObject = JsonSerializer.Deserialize<ExpandoObject>(
            jsonString,
            new JsonSerializerOptions { Converters = { new ExpandoObjectConverter() } });
        return expandoObject;
    }

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, ExpandoObject value)
    {
        var jsonString = JsonSerializer.Serialize(value, new JsonSerializerOptions { Converters = { new ExpandoObjectConverter() } });
        var bsonDocument = BsonDocument.Parse(jsonString);
        BsonSerializer.Serialize(context.Writer, bsonDocument);
    }
}

public class ExpandoObjectConverter : JsonConverter<ExpandoObject>
{
    public override ExpandoObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonDocument = JsonDocument.ParseValue(ref reader);
        return Deserialize(jsonDocument.RootElement);
    }

    public override void Write(Utf8JsonWriter writer, ExpandoObject value, JsonSerializerOptions options)
    {
        var jsonString = JsonSerializer.Serialize(value);
        writer.WriteStringValue(jsonString);
    }

    private ExpandoObject Deserialize(JsonElement element)
    {
        var expandoObject = new ExpandoObject();
        var dictionary = (IDictionary<string, object>)expandoObject;

        foreach (var property in element.EnumerateObject())
        {
            switch (property.Value.ValueKind)
            {
                case JsonValueKind.Object:
                    dictionary[property.Name] = Deserialize(property.Value);
                    break;
                case JsonValueKind.Array:
                    dictionary[property.Name] = property.Value.EnumerateArray().Select(item => Deserialize(item)).ToList();
                    break;
                case JsonValueKind.String:
                    dictionary[property.Name] = property.Value.GetString();
                    break;
                case JsonValueKind.Number:
                    dictionary[property.Name] = property.Value.GetDouble();
                    break;
                case JsonValueKind.True:
                case JsonValueKind.False:
                    dictionary[property.Name] = property.Value.GetBoolean();
                    break;
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    dictionary[property.Name] = null;
                    break;
            }
        }

        return expandoObject;
    }
}


