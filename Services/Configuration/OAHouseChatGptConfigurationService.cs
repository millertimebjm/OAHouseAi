
namespace OAHouseChatGpt.Services.Configuration
{
    public class oAHouseChatGptConfigurationService : IOAHouseChatGptConfiguration
    {
        private string _oADiscordToken;
        private string _openAIApiKey;
        private string _discordBotId;

        public oAHouseChatGptConfigurationService(
            string oADiscordToken,
            string openAIApiKey,
            string discordBotId)
        {
            _oADiscordToken = oADiscordToken;
            _openAIApiKey = openAIApiKey;
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

        public string GetDiscordBotId()
        {
            return _discordBotId;
        }
    }
}