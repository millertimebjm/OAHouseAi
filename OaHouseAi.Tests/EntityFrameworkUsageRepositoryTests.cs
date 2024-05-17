using OAHouseChatGpt.Models.Usages;
using OAHouseChatGpt.Repositories;
using OAHouseChatGpt.Repositories.Usages;
using OAHouseChatGpt.Services.Configuration;

namespace OaHouseAi.Tests;

public class UsageEntityFrameworkTests
{
    [Fact]
    public async void Insert_Equals()
    {
        IOAHouseChatGptConfiguration config = new oAHouseChatGptConfigurationService(
            oADiscordToken: "",
            openAIApiKey: "",
            discordBotId: "",
            databaseConnectionString: "",
            dbContextType: DbContextTypeEnum.InMemory
        );
        IOaHouseAiDbContextFactory dbContextFactory = new OaHouseAiDbContextFactoryRollUp(config);
        IUsageRepository usageRepository = new EntityFrameworkUsageRepository(config, dbContextFactory);
        var usageModel = new UsageModel()
        {
            ModelName = Faker.StringFaker.AlphaNumeric(20),
            TotalTokens = Faker.NumberFaker.Number(1000),
            Username = Faker.StringFaker.AlphaNumeric(20),
            UtcTimestamp = Faker.DateTimeFaker.DateTimeBetweenDays(10),
        };
        await usageRepository.Insert(usageModel);
        var newUsageModel = await usageRepository.GetById(usageModel.Id);
        Assert.Equal(usageModel.Id, newUsageModel.Id);
        Assert.Equal(usageModel.ModelName, newUsageModel.ModelName);
        Assert.Equal(usageModel.TotalTokens, newUsageModel.TotalTokens);
        Assert.Equal(usageModel.UtcTimestamp, newUsageModel.UtcTimestamp);
    }

    [Fact]
    public async void Insert_NotExists()
    {
        IOAHouseChatGptConfiguration config = new oAHouseChatGptConfigurationService(
            oADiscordToken: "",
            openAIApiKey: "",
            discordBotId: "",
            databaseConnectionString: "",
            dbContextType: DbContextTypeEnum.InMemory
        );
        IOaHouseAiDbContextFactory dbContextFactory = new OaHouseAiDbContextFactoryRollUp(config);
        IUsageRepository usageRepository = new EntityFrameworkUsageRepository(config, dbContextFactory);
        var usageModel = new UsageModel()
        {
            ModelName = Faker.StringFaker.AlphaNumeric(20),
            TotalTokens = Faker.NumberFaker.Number(1000),
            Username = Faker.StringFaker.AlphaNumeric(20),
            UtcTimestamp = Faker.DateTimeFaker.DateTimeBetweenDays(10),
        };
        var newUsageModel = await usageRepository.GetById(usageModel.Id);
        Assert.Null(newUsageModel);
    }
}