
namespace OAHouseChatGpt.Services.ChatGpt
{
    public interface IChatGpt
    {
        public Task<ChatGptResponseModel> GetTextCompletion(string request);
    }
}