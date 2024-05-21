
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using OAHouseChatGpt.Models.Usages;
using OAHouseChatGpt.Services.Configuration;

namespace OAHouseChatGpt.Repositories.Usages;

public class MongoDbUsageRepository : IUsageRepository
{
    private readonly Lazy<IMongoCollection<UsageModel>> _collection;
    private readonly IOAHouseChatGptConfiguration _config;
    public MongoDbUsageRepository(
        IOAHouseChatGptConfiguration config)
    {
        _config = config;
        _collection = new Lazy<IMongoCollection<UsageModel>>(() =>
        {
            var client = new MongoClient(_config.DatabaseConnectionString);
            var database = client.GetDatabase("OaHouseAi");
            return database.GetCollection<UsageModel>("Usage");
        });
    }

    public async Task<UsageModel> GetById(string id)
    {
        var queryable = _collection.Value.AsQueryable();
        return await queryable.SingleOrDefaultAsync(_ => _.Id == id);
    }

    public async Task Insert(UsageModel model)
    {
        await _collection.Value.InsertOneAsync(model);
    }
}