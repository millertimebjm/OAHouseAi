
namespace OAHouseChatGpt.Services.Configuration
{
    public class oAHouseChatGptConfigurationService : IOAHouseChatGptConfiguration
    {
        private string _oADiscordToken;
        private string _openAIApiKey;
        private string _discordBotUsername;
        private string _discordBotId;

        public oAHouseChatGptConfigurationService(
            string oADiscordToken,
            string openAIApiKey,
            string discordBotUsername,
            string discordBotId)
        {
            _oADiscordToken = oADiscordToken;
            _openAIApiKey = openAIApiKey;
            _discordBotUsername = discordBotUsername;
            _discordBotId = discordBotId;
        }

        public string GetOADiscordToken()
        {
            return _oADiscordToken;
        }

        public string GetOpenAIApiKey()
        {
            return _openAIApiKey;
        }

        public string GetDiscordBotUsername()
        {
            return _discordBotUsername;
        }

        public string GetDiscordBotId()
        {
            return _discordBotId;
        }
    }
}