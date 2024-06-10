using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using OaHouseAi.Configuration.Services.Interfaces;
using Serilog;
using OaHouseAi.Discord.Services.Interfaces;
using OaHouseAi.Discord.Models;

namespace OaHouseAi.Discord.Services;

public class OaDiscordHttpService : IOaDiscordHttp
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOAHouseChatGptConfiguration _config;

    public OaDiscordHttpService(
        IHttpClientFactory httpClientFactory,
        IOAHouseChatGptConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
    }

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public async Task<DiscordUser> GetUserAsync(string userId)
    {
        var url = $"https://discord.com/api/v10/users/{userId}";
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", _config.OADiscordToken);
        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            // Handle error response appropriately
            Log.Error("OaDiscordHttpService: GetUserAsync: There was an error getting the user {s1} {s2}", userId, response.ReasonPhrase);
            return null;
        }
        Log.Debug("OaDiscordHttpService: GetUserAsync: Just before readfromjsonasync");
        return await response.Content.ReadFromJsonAsync<DiscordUser>(DiscordUser.GetJsonSerializerOptions());
    }

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public async Task<DiscordChannel> GetChannelAsync(string channelId)
    {
        var url = $"https://discord.com/api/v10/channels/{channelId}";
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", _config.OADiscordToken);
        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            // Handle error response appropriately
            Log.Error("OaDiscordHttpService: GetChannelAsync: There was an error getting the channel {s1} {s2}", channelId, response.ReasonPhrase);
            return null;
        }
        Log.Debug("OaDiscordHttpService: GetUserAsync: Just before readfromjsonasync");
        return await response.Content.ReadFromJsonAsync<DiscordChannel>(DiscordChannel.GetJsonSerializerOptions());
    }

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public async Task<bool> TriggerTypingAsync(string channelId)
    {
        var url = $"https://discord.com/api/v10/channels/{channelId}/typing";
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", _config.OADiscordToken);
        var response = await httpClient.PostAsync(url, null);
        if (!response.IsSuccessStatusCode)
        {
            // Handle error response appropriately
            Log.Error("OaDiscordHttpService: GetChannelAsync: There was an error triggering the typingon channel {s1} {s2}", channelId, response.ReasonPhrase);
            return false;
        }
        return true;
    }

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public async Task<DiscordMessage> GetMessageAsync(string channelId, string messageId)
    {
        var url = $"https://discord.com/api/v10/channels/{channelId}/messages/{messageId}";
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", _config.OADiscordToken);
        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            // Handle error response appropriately
            Log.Error("OaDiscordHttpService: GetChannelAsync: Getting the message channel id: {s1}, message id: {s2}", channelId, response.ReasonPhrase);
            return null;
        }
        Log.Debug("OaDiscordHttpService: GetMessageAsync: {s}", await response.Content.ReadAsStringAsync());
        return await response.Content.ReadFromJsonAsync<DiscordMessage>(DiscordMessage.GetJsonSerializerOptions());
    }

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public async Task<bool> SendMessageAsync(string channelId, string content)
    {
        var url = $"https://discord.com/api/v10/channels/{channelId}/messages";
        var payload = new DiscordMessageSend()
        {
            Content = content
        };
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", _config.OADiscordToken);
        var response = await httpClient.PostAsJsonAsync(url, payload, DiscordMessageSend.GetJsonSerializerOptions());
        if (!response.IsSuccessStatusCode)
        {
            Log.Error("OaDiscordHttpService: SendMessageAsync: There was an error sending message on channel {s1}\n{s2}\n {s3}", channelId, content, response.ReasonPhrase);
            return false;
        }
        return true;
    }

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public async Task<bool> SendMessageAsync(string channelId, string content, string referenceMessageId)
    {
        var url = $"https://discord.com/api/v10/channels/{channelId}/messages";
        var payload = new DiscordMessageSend()
        {
            Content = content,
            ReferenceMessage = new DiscordMessageSendReference() { MessageId = referenceMessageId },
        };
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", _config.OADiscordToken);
        Log.Debug("OaDiscordHttpService: SendMessageAsync: {s1}\n{s2}", url, payload.Serialize());
        var response = await httpClient.PostAsJsonAsync(url, payload, DiscordMessageSend.GetJsonSerializerOptions());
        if (!response.IsSuccessStatusCode)
        {
            Log.Error("OaDiscordHttpService: SendMessageAsync: There was an error sending chunk message on channel {s1}\n{s2}\n{s3}", channelId, content, response.ReasonPhrase);
            return false;
        }
        return true;
    }
}