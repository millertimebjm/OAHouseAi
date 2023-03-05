
namespace OAHouseChatGpt.Services.Configuration
{
    public interface IOAHouseChatGptConfiguration
    {
        public ulong GetOADiscordChannelId();
        public string GetOADiscordToken();
        public string GetOpenAIApiKey();
    }
}