using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Azure.Core;
using OAHouseChatGpt.Models.Usages;
//using OAHouseChatGpt.Repositories.Usages;
using OAHouseChatGpt.Services.Configuration;
using Serilog;

namespace OAHouseChatGpt.Services.ChatGpt
{
    public class ChatGptService : IChatGpt
    {
        private const string _baseUrl = "https://api.openai.com";
        private const string _resource = "/v1/chat/completions";
        private readonly string _openAIApiKey;
        private readonly IHttpClientFactory _httpClientFactory;

        public ChatGptService(
            IOAHouseChatGptConfiguration configurationService,
            IHttpClientFactory httpClientFactory)
        {
            _openAIApiKey = configurationService.OpenAIApiKey;
            Log.Debug("ChatGPTService: OpenAI Api Key: {_openAIApiKey}", _openAIApiKey);
            _httpClientFactory = httpClientFactory;
        }

        [RequiresUnreferencedCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
        [RequiresDynamicCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
        public async Task<ChatGptResponseModel> GetTextCompletion(
            string text,
            IEnumerable<ChatGptMessageModel> context = null)
        {
            Log.Debug("ChatGPTService: Text completion text: {text}", text);
            if (string.IsNullOrWhiteSpace(_openAIApiKey)) throw new ArgumentException("OpenAiApiKey is not set.");

            Log.Debug("ChatGptService: Sending HttpClient request...");
            var body = CreateBody(text, context);
            Log.Debug("GetTextCompletion: SendChatGptRequest post data: {postData}", body.Serialize());
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _openAIApiKey);

            var response = await httpClient.PostAsJsonAsync(
                _baseUrl + _resource, 
                body,
                ChatGptBodyModel.GetJsonSerializerOptions());
            if (!response.IsSuccessStatusCode)
            {
                Log.Error("HttpClient Response error: {s1}, {s2}", response.StatusCode, response.ReasonPhrase);
                return null;
            }
            return await response.Content.ReadFromJsonAsync<ChatGptResponseModel>(
                ChatGptResponseModel.GetJsonSerializerOptions());
        }

        private ChatGptBodyModel CreateBody(
            string text,
            IEnumerable<ChatGptMessageModel> context)
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