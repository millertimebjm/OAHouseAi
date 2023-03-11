
namespace OAHouseChatGpt.Services.Configuration
{
    public interface IOAHouseChatGptConfiguration
    {
        public string GetOADiscordToken();
        public string GetOpenAIApiKey();
        public string GetDiscordBotId();
    }
}