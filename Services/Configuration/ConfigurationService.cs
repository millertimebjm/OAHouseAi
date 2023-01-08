
namespace OAHouseChatGpt.Services.Configuration
{
    public class ConfigurationService : IConfiguration
    {
        private ulong _oADiscordChannel;
        private string _oADiscordToken;
        private string _openAIServiceApiKey;

        public ConfigurationService(ulong oADiscordChannel, string oADiscordToken, string openAIServiceApiKey)
        {
            _oADiscordChannel = oADiscordChannel;
            _oADiscordToken = oADiscordToken;
            _openAIServiceApiKey = openAIServiceApiKey;
        }

        public ulong GetOADiscordChannelId()
        {
            return _oADiscordChannel;
        }

        public string GetOADiscordToken()
        {
            return _oADiscordToken;
        }

        public string GetOpenAIServiceApiKey()
        {
            return _openAIServiceApiKey;
        }
    }
}