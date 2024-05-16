
using System.Diagnostics.CodeAnalysis;

namespace OAHouseChatGpt.Services.ChatGpt
{
    public interface IChatGpt
    {
        [RequiresUnreferencedCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
        [RequiresDynamicCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
        public Task<ChatGptResponseModel> GetTextCompletion(
            string request,
            IEnumerable<ChatGptMessageModel> context = null);
    }
}