
using MongoDB.Driver;
using OAHouseChatGpt.Models.Usages;
using OAHouseChatGpt.Services.Configuration;

namespace OAHouseChatGpt.Repositories.Usages;

public class EntityFrameworkUsageRepository : IUsageRepository
{
    private readonly IOAHouseChatGptConfiguration _config;
    private readonly Lazy<OaHouseAiDbContext> _dbContext;
    public EntityFrameworkUsageRepository(IOAHouseChatGptConfiguration config)
    {
        _config = config;
        
        _dbContext = new Lazy<OaHouseAiDbContext>(() => {
            var client = new MongoClient(_config.GetDatabaseConnectionString());
            return OaHouseAiDbContext.Create(client.GetDatabase("OaHouseAi"));
        });
    }

    public async Task Insert(UsageModel model)
    {
        await _dbContext.Value.AddAsync(model);
        await _dbContext.Value.SaveChangesAsync();
    }
}