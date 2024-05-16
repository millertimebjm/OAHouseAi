
namespace OAHouseChatGpt.Services.Configuration
{
    public interface IOAHouseChatGptConfiguration
    {
        public string OADiscordToken { get; }
        public string OpenAIApiKey { get; }
        public string DiscordBotId { get; }
        public string DatabaseConnectionString { get; }
        public string DbContextType { get; }
    }
}