using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OAHouseChatGpt.Services.Configuration;

namespace OAHouseChatGpt.Services.ChatGpt
{
    public class ChatGptService : IChatGpt
    {
        private readonly OpenAIService _openAIService;
        private readonly IConfiguration _configurationService;
        public ChatGptService(IConfiguration configurationService)
        {
            _configurationService = configurationService;
            _openAIService = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = _configurationService.GetOpenAIServiceApiKey(),
            });
        }

        public async Task<string> GetTextCompletion(string request)
        {
            var completionResult = await _openAIService.Completions.CreateCompletion(new CompletionCreateRequest()
            {
                Prompt = request,
                MaxTokens = 5
            }, Models.Davinci);

            if (completionResult.Successful)
            {
                return completionResult.Choices.FirstOrDefault()?.Text ?? "No Response";
            }
            else
            {
                if (completionResult.Error == null)
                {
                    return "Unknown Error";
                }
                return $"{completionResult.Error.Code}: {completionResult.Error.Message}";
            }
        }
    }
}