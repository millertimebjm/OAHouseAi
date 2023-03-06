
namespace OAHouseChatGpt.Services.Configuration
{
    public class oAHouseChatGptConfigurationService : IOAHouseChatGptConfiguration
    {
        private ulong _oADiscordGuild;
        private ulong _oADiscordChannel;
        private string _oADiscordToken;
        private string _openAIApiKey;
        private string _discordBotUsername;

        public oAHouseChatGptConfigurationService(
            ulong oADiscordGuild,
            ulong oADiscordChannel,
            string oADiscordToken,
            string openAIApiKey,
            string discordBotUsername)
        {
            _oADiscordGuild = oADiscordGuild;
            _oADiscordChannel = oADiscordChannel;
            _oADiscordToken = oADiscordToken;
            _openAIApiKey = openAIApiKey;
            _discordBotUsername = discordBotUsername;
        }

        public ulong GetOADiscordGuildId()
        {
            return _oADiscordGuild;
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

        public string GetDiscordBotUsername()
        {
            return _discordBotUsername;
        }
    }
}