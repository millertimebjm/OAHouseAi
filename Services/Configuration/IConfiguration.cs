
namespace OAHouseChatGpt.Services.Configuration
{
    public interface IConfiguration
    {
        public ulong GetOADiscordChannelId();
        public string GetOADiscordToken();
        public string GetOpenAIServiceApiKey();
    }
}