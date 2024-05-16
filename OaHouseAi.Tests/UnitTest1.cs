using OAHouseChatGpt.Repositories;
using OAHouseChatGpt.Services.Configuration;

namespace OaHouseAi.Tests;

public class UsageEntityFrameworkTests
{


    [Fact]
    public void Insert_Equals()
    {
        IOAHouseChatGptConfiguration config = new oAHouseChatGptConfigurationService(
            oADiscordToken: "",
            openAIApiKey: "",
            discordBotId: "",
            databaseConnectionString: "",
            dbContextType: "InMemory"
        );
        IOaHouseAiDbContextFactory dbContextFactory = new OaHouseAiDbContextFactoryRollUp(config);
        var context = dbContextFactory.GetDbContext(config.DbContextType);

    }
}