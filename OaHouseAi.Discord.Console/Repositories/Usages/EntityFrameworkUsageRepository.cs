
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using OAHouseChatGpt.Models.Usages;
using OAHouseChatGpt.Services.Configuration;

namespace OAHouseChatGpt.Repositories.Usages;

public class EntityFrameworkUsageRepository : IUsageRepository
{
    private readonly IOAHouseChatGptConfiguration _config;
    private readonly Lazy<OaHouseAiDbContext> _dbContext;
    private readonly IOaHouseAiDbContextFactory _oaHouseAiDbContextFactory;
    public EntityFrameworkUsageRepository(
        IOAHouseChatGptConfiguration config,
        IOaHouseAiDbContextFactory oaHouseAiDbContextFactory)
    {
        _config = config;
        _oaHouseAiDbContextFactory = oaHouseAiDbContextFactory;

        _dbContext = new Lazy<OaHouseAiDbContext>(_oaHouseAiDbContextFactory.GetDbContext(config.DbContextType));
    }

    public async Task<UsageModel> GetById(string id)
    {
        return await _dbContext.Value.Usages.SingleOrDefaultAsync(_ => _.Id == id);
    }

    public async Task Insert(UsageModel model)
    {
        await _dbContext.Value.AddAsync(model);
        await _dbContext.Value.SaveChangesAsync();
    }
}