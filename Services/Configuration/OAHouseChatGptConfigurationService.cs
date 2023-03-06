
namespace OAHouseChatGpt.Services.Configuration
{
    public class oAHouseChatGptConfigurationService : IOAHouseChatGptConfiguration
    {
        private string _oADiscordToken;
        private string _openAIApiKey;
        private string _discordBotUsername;

        public oAHouseChatGptConfigurationService(
            string oADiscordToken,
            string openAIApiKey,
            string discordBotUsername)
        {
            _oADiscordToken = oADiscordToken;
            _openAIApiKey = openAIApiKey;
            _discordBotUsername = discordBotUsername;
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
    }
}