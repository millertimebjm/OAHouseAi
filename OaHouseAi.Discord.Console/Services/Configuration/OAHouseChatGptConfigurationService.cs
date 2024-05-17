
using MongoDB.Driver;
using OAHouseChatGpt.Repositories;

namespace OAHouseChatGpt.Services.Configuration
{
    public class oAHouseChatGptConfigurationService : IOAHouseChatGptConfiguration
    {
        public string OADiscordToken { get; }
        public string OpenAIApiKey { get; }
        public string DiscordBotId { get; }
        public string DatabaseConnectionString { get; }
        public DbContextTypeEnum DbContextType { get; }

        public oAHouseChatGptConfigurationService(
            string oADiscordToken,
            string openAIApiKey,
            string discordBotId,
            string databaseConnectionString,
            DbContextTypeEnum dbContextType)
        {
            OADiscordToken = oADiscordToken;
            OpenAIApiKey = openAIApiKey;
            DiscordBotId = discordBotId;
            DatabaseConnectionString = databaseConnectionString;
            DbContextType = dbContextType;
        }
    }
}