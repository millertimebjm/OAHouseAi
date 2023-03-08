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
            var messages = new List<string>();
            messages.Add(text);
            messages.AddRange(context ?? new List<string>());
            var body = new
            {
                model = "gpt-3.5-turbo",
                messages = messages.Select(_ => new
                {
                    role = "user",
                    content = _,
                }),
            };
            request.AddJsonBody(body);
            //var response = await client.ExecuteAsync<dynamic>(request);
            var response = await client.ExecuteAsync<ChatGptResponseModel>(request);
            var model = response.Data;
            // var options = new JsonSerializerOptions
            // {
            //     PropertyNameCaseInsensitive = true
            // };
            // var model = JsonSerializer.Deserialize<ChatGptResponseModel>(response.Content, options);
            return model;
        }
    }
}