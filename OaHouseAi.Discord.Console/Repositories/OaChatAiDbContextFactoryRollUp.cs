
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using OAHouseChatGpt.Services.ChatGpt;
using OAHouseChatGpt.Services.Configuration;

namespace OAHouseChatGpt.Repositories;

public class OaHouseAiDbContextFactoryRollUp : IOaHouseAiDbContextFactory
{
    private readonly IOAHouseChatGptConfiguration _config;
    public OaHouseAiDbContextFactoryRollUp(IOAHouseChatGptConfiguration config)
    {
        _config = config;
    }

    [RequiresDynamicCode("")]
    [RequiresUnreferencedCode("")]
    public OaHouseAiDbContext GetDbContext(DbContextTypeEnum type)
    {
        if (type == DbContextTypeEnum.MongoDb)
        {
            var client = new MongoClient(_config.DatabaseConnectionString);
            return OaHouseAiDbContext.Create(client.GetDatabase("OaHouseAi"));
        }
        else if (type == DbContextTypeEnum.InMemory)
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