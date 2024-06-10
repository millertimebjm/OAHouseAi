using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using OaHouseAi.Repository.Usages.Models; 
using OaHouseAi.Repository.Usages.Services.Interfaces;
using OaHouseAi.Configuration.Services.Interfaces;

namespace OaHouseAi.Repository.Usages.Services;

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
        return JsonSerializer.Deserialize<UsageModel>(resultJson,
            UsageModel.GetJsonSerializerOptions());
    }

    public async Task<string> Upsert(UsageModel model)
    {
        var modelJson = model.Serialize();
        var modelBson = BsonDocument.Parse(modelJson);
        if (model.Id is null)
        {
            modelBson.Remove("_id");
            await _collection.Value.InsertOneAsync(modelBson);
            return modelBson["_id"].ToString();
        }
        var filter = Builders<BsonDocument>.Filter.Eq("_id", model.Id);
        await _collection.Value.UpdateOneAsync(filter, BsonDocument.Parse(modelJson));
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