
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using OAHouseChatGpt.Models.Usages;
using OAHouseChatGpt.Services.Configuration;
using Serilog;

namespace OAHouseChatGpt.Repositories.Usages;

public class MongoDbUsageRepository : IUsageRepository
{
    private readonly Lazy<IMongoCollection<BsonDocument>> _collection;
    private readonly IOAHouseChatGptConfiguration _config;
    public MongoDbUsageRepository(
        IOAHouseChatGptConfiguration config)
    {
        _config = config;
        _collection = new Lazy<IMongoCollection<BsonDocument>>(() =>
        {
            var client = new MongoClient(_config.DatabaseConnectionString);
            var database = client.GetDatabase(_config.DatabaseName);
            return database.GetCollection<BsonDocument>("Usage");
        });
    }

    public async Task<UsageModel> GetById(string id)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
        var result = _collection.Value.Find(filter).FirstOrDefault();
        var resultJson = result.ToJson();
        return JsonSerializer.Deserialize<UsageModel>(resultJson);
    }

    public async Task<string> Upsert(UsageModel model)
    {
        var properties = new Dictionary<string, object>()
        {
            { nameof(UsageModel.ModelName), model.ModelName },
            { nameof(UsageModel.Username), model.Username },
            { nameof(UsageModel.TotalTokens), model.TotalTokens },
            { nameof(UsageModel.UtcTimestamp), model.UtcTimestamp }
        };
        if (model.Id is null)
        {
            var modelBson = new BsonDocument(properties);
            await _collection.Value.InsertOneAsync(modelBson);
            return modelBson["_id"].ToString();
        }
        var filter = Builders<BsonDocument>.Filter.Eq("_id", model.Id);
        properties.Add(nameof(UsageModel.Id), model.Id);
        await _collection.Value.UpdateOneAsync(filter, new BsonDocument(properties));
        return model.Id;
    }

    public async Task<string> Insert(string modelName, string username, int totalTokens)
    {
        return await Upsert(new UsageModel()
        {
            ModelName = modelName,
            Username = username,
            TotalTokens = totalTokens,
        });
    }
}