
namespace OAHouseChatGpt.Services.Configuration
{
    public class oAHouseChatGptConfigurationService : IOAHouseChatGptConfiguration
    {
        private ulong _oADiscordChannel;
        private string _oADiscordToken;
        private string _openAIApiKey;

        public oAHouseChatGptConfigurationService(ulong oADiscordChannel, string oADiscordToken, string openAIApiKey)
        {
            _oADiscordChannel = oADiscordChannel;
            _oADiscordToken = oADiscordToken;
            _openAIApiKey = openAIApiKey;
        }

        public ulong GetOADiscordChannelId()
        {
            return _oADiscordChannel;
        }

        public string GetOADiscordToken()
        {
            return _oADiscordToken;
        }

        public string GetOpenAIApiKey()
        {
            return _openAIApiKey;
        }
    }
}