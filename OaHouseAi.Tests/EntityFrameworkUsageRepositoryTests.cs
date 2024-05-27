using OAHouseChatGpt.Models.Usages;
using OAHouseChatGpt.Repositories;
using OAHouseChatGpt.Repositories.Usages;
using OAHouseChatGpt.Services.Configuration;

namespace OaHouseAi.Tests;

public class UsageEntityFrameworkTests
{
    // [Fact]
    // public async void Insert_Equals()
    // {
    //     IOAHouseChatGptConfiguration config = new oAHouseChatGptConfigurationService(
    //         oADiscordToken: "",
    //         openAIApiKey: "",
    //         discordBotId: "",
    //         databaseConnectionString: "",
    //         loggingCollectionName: "",
    //         dbContextType: DbContextTypeEnum.InMemory
    //     );
    //     IOaHouseAiDbContextFactory dbContextFactory = new OaHouseAiDbContextFactoryRollUp(config);
    //     IUsageRepository usageRepository = new EntityFrameworkUsageRepository(config, dbContextFactory);
    //     var usageModel = new UsageModel()
    //     {
    //         ModelName = Faker.Lorem.Sentence(5),
    //         TotalTokens = Faker.RandomNumber.Next(),
    //         Username = Faker.Internet.UserName(),
    //         UtcTimestamp = Faker.Identification.DateOfBirth(),
    //     };
    //     await usageRepository.Insert(usageModel);
    //     var newUsageModel = await usageRepository.GetById(usageModel.Id);
    //     Assert.Equal(usageModel.Id, newUsageModel.Id);
    //     Assert.Equal(usageModel.ModelName, newUsageModel.ModelName);
    //     Assert.Equal(usageModel.TotalTokens, newUsageModel.TotalTokens);
    //     Assert.Equal(usageModel.UtcTimestamp, newUsageModel.UtcTimestamp);
    // }

    // [Fact]
    // public async Task Insert_ByModel_NotExists()
    // {
    //     IOAHouseChatGptConfiguration config = new oAHouseChatGptConfigurationService(
    //         oADiscordToken: "",
    //         openAIApiKey: "",
    //         discordBotId: "",
    //         databaseConnectionString: "",
    //         loggingCollectionName: "",
    //         databaseName: "",
    //         dbContextType: DbContextTypeEnum.InMemory
    //     );
    //     IOaHouseAiDbContextFactory dbContextFactory = new OaHouseAiDbContextFactoryRollUp(config);
    //     IUsageRepository usageRepository = new EntityFrameworkUsageRepository(config, dbContextFactory);
    //     var usageModel = new UsageModel()
    //     {
    //         ModelName = Faker.Lorem.Sentence(5),
    //         TotalTokens = Faker.RandomNumber.Next(),
    //         Username = Faker.Internet.UserName(),
    //         UtcTimestamp = DateTime.UtcNow,
    //     };
    //     var newUsageModel = usageRepository.Insert(usageModel);
    //     var newnewUsageModel = await usageRepository.GetById(newUsageModel.Id.ToString());
    //     Assert.Null(newnewUsageModel);
    // }

    // [Fact]
    // public async Task Insert_ByParameters_NotExists()
    // {
    //     IOAHouseChatGptConfiguration config = new oAHouseChatGptConfigurationService(
    //         oADiscordToken: "",
    //         openAIApiKey: "",
    //         discordBotId: "",
    //         databaseConnectionString: "",
    //         loggingCollectionName: "",
    //         databaseName: "",
    //         dbContextType: DbContextTypeEnum.InMemory
    //     );
    //     IOaHouseAiDbContextFactory dbContextFactory = new OaHouseAiDbContextFactoryRollUp(config);
    //     IUsageRepository usageRepository = new EntityFrameworkUsageRepository(config, dbContextFactory);
    //     var newUsageModel = usageRepository.Insert(modelName: Faker.Lorem.Sentence(5),
    //         username: Faker.Internet.UserName(),
    //         totalTokens: Faker.RandomNumber.Next());
    //     var newUsageModel = await usageRepository.GetById(newUsageModel.Id.ToString());
    //     Assert.Null(newUsageModel);
    // }
}