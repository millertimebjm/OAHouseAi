using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;
using OAHouseChatGpt.Services.Configuration;
using RestSharp;

namespace OAHouseChatGpt.Services.ChatGpt
{
    public class ChatGptService : IChatGpt
    {
        private readonly string _openAIApiKey;
        public ChatGptService(IOAHouseChatGptConfiguration configurationService)
        {
            _openAIApiKey = configurationService.GetOpenAIApiKey();
        }

        public async Task<ChatGptResponseModel> GetTextCompletion(
            string text,
            IEnumerable<ChatGptMessageModel> context = null)
        {
            if (string.IsNullOrWhiteSpace(_openAIApiKey)) return null;

            var client = new RestClient("https://api.openai.com");
            var request = new RestRequest("/v1/chat/completions", Method.Post);
            request.AddHeader("Authorization", $"Bearer {_openAIApiKey}");
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(CreateBody(text, context));
            var response = await client.ExecuteAsync<ChatGptResponseModel>(request);
            // var options = new JsonSerializerOptions
            // {
            //     PropertyNameCaseInsensitive = true
            // };
            // var model = JsonSerializer.Deserialize<ChatGptResponseModel>(response.Content, options);
            return response.Data;
        }

        private ChatGptBodyModel CreateBody(string text, IEnumerable<ChatGptMessageModel> context)
        {
            var messages = new List<ChatGptMessageModel>();
            messages.AddRange(context ?? new List<ChatGptMessageModel>());
            messages.Add(new ChatGptMessageModel()
            {
                Role = "user",
                Content = text,
            });
            var body = new ChatGptBodyModel()
            {
                Model = "gpt-3.5-turbo",
                Messages = messages,
            };
            return body;
        }
    }
}