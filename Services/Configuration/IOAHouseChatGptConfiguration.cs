
namespace OAHouseChatGpt.Services.Configuration
{
    public interface IOAHouseChatGptConfiguration
    {
        public ulong GetOADiscordGuildId();
        public ulong GetOADiscordChannelId();
        public string GetOADiscordToken();
        public string GetOpenAIApiKey();
        public string GetDiscordBotUsername();
    }
}