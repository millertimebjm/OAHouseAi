using OaHouseAi.Configuration.Services.Interfaces;

namespace OaHouseAi.Configuration.Services;

public class oAHouseChatGptConfigurationService : IOAHouseChatGptConfiguration
{
    public string OADiscordToken { get; }
    public string OpenAIApiKey { get; }
    public string DiscordBotId { get; }
    public string DatabaseConnectionString { get; }
    public string LoggingCollectionName { get; }
    public string DatabaseName { get; }
    public string DatabaseServer { get; }
    public string DbContextType { get; }

    public oAHouseChatGptConfigurationService(
        string oADiscordToken,
        string openAIApiKey,
        string discordBotId,
        string databaseConnectionString,
        string loggingCollectionName,
        string databaseName,
        string databaseServer,
        string dbContextType)
    {
        OADiscordToken = oADiscordToken;
        OpenAIApiKey = openAIApiKey;
        DiscordBotId = discordBotId;
        DatabaseConnectionString = databaseConnectionString;
        LoggingCollectionName = loggingCollectionName;
        DatabaseName = databaseName;
        DatabaseServer = databaseServer;
        DbContextType = dbContextType;
    }
}
