
using MongoDB.Driver;

namespace OAHouseChatGpt.Services.Configuration
{
    public class oAHouseChatGptConfigurationService : IOAHouseChatGptConfiguration
    {
        private string _oADiscordToken;
        private string _openAIApiKey;
        private string _discordBotId;
        private string _databaseConnectionString;

        public oAHouseChatGptConfigurationService(
            string oADiscordToken,
            string openAIApiKey,
            string discordBotId,
            string databaseConnectionString)
        {
            _oADiscordToken = oADiscordToken;
            _openAIApiKey = openAIApiKey;
            _discordBotId = discordBotId;
            _databaseConnectionString = databaseConnectionString;
        }

        public string GetOADiscordToken() => _oADiscordToken;

        public string GetOpenAIApiKey() => _openAIApiKey;

        public string GetDiscordBotId() => _discordBotId;

        public string GetDatabaseConnectionString() => _databaseConnectionString;
    }
}