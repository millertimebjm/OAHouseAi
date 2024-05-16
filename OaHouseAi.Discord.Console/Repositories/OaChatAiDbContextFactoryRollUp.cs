
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using OAHouseChatGpt.Services.ChatGpt;
using OAHouseChatGpt.Services.Configuration;

namespace OAHouseChatGpt.Repositories;

public class OaHouseAiDbContextFactoryRollUp : IOaHouseAiDbContextFactory
{
    private readonly oAHouseChatGptConfigurationService _config;
    public OaHouseAiDbContextFactoryRollUp(oAHouseChatGptConfigurationService config)
    {
        _config = config;
    }

    public OaHouseAiDbContext GetDbContext(string type)
    {
        if (type == "MongoDb")
        {
            var client = new MongoClient(_config.DatabaseConnectionString);
            return OaHouseAiDbContext.Create(client.GetDatabase("OaHouseAi"));
        }
        else if (type == "InMemory")
        {
            var options = new DbContextOptionsBuilder<OaHouseAiDbContext>()
                .UseInMemoryDatabase("OaHouseAi")
                .Options;
            var context = new OaHouseAiDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        throw new NotImplementedException();
    }
}