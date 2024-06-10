using System.Diagnostics.CodeAnalysis;

namespace OaHouseAi.Discord.Services.Interfaces;

public interface IOaDiscord
{
    [RequiresUnreferencedCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
    [RequiresDynamicCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
    Task Start();

    [RequiresUnreferencedCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
    [RequiresDynamicCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
    Task SendLongMessage(
        string channelId,
        string referenceMessageId,
        string messageContent,
        int totalTokens,
        int maxMessageLength = 2000);

    [RequiresUnreferencedCode("Calls OAHouseChatGpt.Services.Discord.DiscordIdentify.Serialize()")]
    [RequiresDynamicCode("Calls OAHouseChatGpt.Services.Discord.DiscordIdentify.Serialize()")]
    Task Identify();
}
