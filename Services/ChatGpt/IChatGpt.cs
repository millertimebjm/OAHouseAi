
namespace OAHouseChatGpt.Services.ChatGpt
{
    public interface IChatGpt
    {
        public Task<string> GetTextCompletion(string request);
    }
}