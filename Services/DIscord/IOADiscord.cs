using System.Diagnostics.CodeAnalysis;
using OAHouseChatGpt.Services.ChatGpt;

namespace OAHouseChatGpt.Services.OADiscord
{
    public interface IOaDiscord
    {
        [RequiresUnreferencedCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
        [RequiresDynamicCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
        Task Start();
    }
}