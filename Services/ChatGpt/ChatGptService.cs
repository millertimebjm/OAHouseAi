using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;
using OAHouseChatGpt.Services.Configuration;
using RestSharp;

namespace OAHouseChatGpt.Services.ChatGpt
{
    public class ChatGptService : IChatGpt
    {
        private readonly string OpenAIApiKey;
        public ChatGptService(IOAHouseChatGptConfiguration configurationService)
        {
            OpenAIApiKey = configurationService.GetOpenAIApiKey();
        }

        public async Task<ChatGptResponseModel> GetTextCompletion(string text, IEnumerable<string> context = null)
        {
            if (string.IsNullOrWhiteSpace(OpenAIApiKey)) return null;

            var client = new RestClient("https://api.openai.com");
            var request = new RestRequest("/v1/chat/completions", Method.Post);
            request.AddHeader("Authorization", $"Bearer {OpenAIApiKey}");
            request.AddHeader("Content-Type", "application/json");
            var body = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = text,
                    },
                },
            };
            foreach (var message in context ?? new List<string>())
            {
                body.messages.Append(new
                {
                    role = "user",
                    content = message,
                });
            }
            request.AddJsonBody(body);
            var response = await client.ExecuteAsync<dynamic>(request);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var model = JsonSerializer.Deserialize<ChatGptResponseModel>(response.Content, options);
            return model;
        }
    }
}