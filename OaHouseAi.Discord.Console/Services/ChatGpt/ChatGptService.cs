using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Azure.Core;
using OAHouseChatGpt.Models.Usages;
using OAHouseChatGpt.Repositories.Usages;
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
        private readonly Lazy<JsonSerializerOptions> _chatGptBodyModelJsonSerializerOptions;
        private readonly Lazy<JsonSerializerOptions> _chatGptResponseModelJsonSerializerContext;
        
        public ChatGptService(
            IOAHouseChatGptConfiguration configurationService,
            IHttpClientFactory httpClientFactory)
        {
            _openAIApiKey = configurationService.GetOpenAIApiKey();
            Log.Debug("ChatGPTService: OpenAI Api Key: {_openAIApiKey}", _openAIApiKey);
            _httpClientFactory = httpClientFactory;
            _chatGptBodyModelJsonSerializerOptions = new Lazy<JsonSerializerOptions>(() => 
                new JsonSerializerOptions()
                {
                    TypeInfoResolver = new ChatGptBodyModelJsonSerializerContext(),
                });
            _chatGptResponseModelJsonSerializerContext = new Lazy<JsonSerializerOptions>(() =>
                new JsonSerializerOptions()
                {
                    TypeInfoResolver = new ChatGptResponseModelJsonSerializerContext()
                });
        }

        [RequiresUnreferencedCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
        [RequiresDynamicCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
        public async Task<ChatGptResponseModel> GetTextCompletion(
            string text,
            IEnumerable<ChatGptMessageModel> context = null)
        {
            Log.Debug("ChatGPTService: Text completion text: {text}", text);
            if (string.IsNullOrWhiteSpace(_openAIApiKey)) return null;

            return await HttpClient_SendChatGptRequest(text, context);
        }

        [RequiresUnreferencedCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
        [RequiresDynamicCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
        private async Task<ChatGptResponseModel> HttpClient_SendChatGptRequest(
            string text, 
            IEnumerable<ChatGptMessageModel> context)
        {
            Log.Debug("ChatGptService: Sending HttpClient request...");
            var postData = JsonSerializer.Serialize(
                CreateBody(text, context),
                _chatGptBodyModelJsonSerializerOptions.Value);
            var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl + _resource)
            {
                Content = new StringContent(
                    postData,
                    Encoding.UTF8,
                    "application/json")
            };
            Log.Debug("GetTextCompletion: SendChatGptRequest post data: {postData}", postData);
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _openAIApiKey);
            var httpResponseMessage = await httpClient.SendAsync(request);
            // var httpResponseMessage = await httpClient.PostAsync(
            //     _resource,
            //     new StringContent(postData, Encoding.UTF8, "application/json"));
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                Log.Error($"HttpClient Request error: {request}");
                Log.Error($"HttpClient Response error: {httpResponseMessage}");
                return null;
            }
            var data = await httpResponseMessage.Content.ReadAsStringAsync();
            
            var response = JsonSerializer.Deserialize<ChatGptResponseModel>(
                data,
                new JsonSerializerOptions()
                {
                    TypeInfoResolver = new ChatGptResponseModelJsonSerializerContext()
                });

            return response;
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