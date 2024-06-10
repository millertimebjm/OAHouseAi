using Microsoft.EntityFrameworkCore.Internal;
//using OAHouseChatGpt.Models.Usages;
//using OAHouseChatGpt.Repositories;
//using OaHouseAi.Repository
using OaHouseAi.Configuration.Services.Interfaces;
using OaHouseAi.Configuration.Services;
using OaHouseAi.Repository.Contexts.Interfaces;
using OaHouseAi.Repository.Contexts;

namespace OaHouseAi.Tests;

public class OaHouseAiDbContextFactoryRollUpTests
{
    [Fact]
    public void DbContextFactory_NoException()
    {
        IOAHouseChatGptConfiguration config = new oAHouseChatGptConfigurationService(
            oADiscordToken: "",
            openAIApiKey: "",
            discordBotId: "",
            databaseConnectionString: "mongodb://test.test.com:27017/OaHouseAi",
            loggingCollectionName: "",
            databaseName: "",
            databaseServer: "",
            dbContextType: DbContextTypeEnum.InMemory.ToString()
        );
        IOaHouseAiDbContextFactory dbContextFactory = new OaHouseAiDbContextFactoryRollUp(config);
        var values = Enum.GetValues(typeof(DbContextTypeEnum)).Cast<DbContextTypeEnum>();
        foreach (var value in values)
        {
            var dbContext = dbContextFactory.GetDbContext(value);
            Assert.NotNull(dbContext);
        }
    }
}