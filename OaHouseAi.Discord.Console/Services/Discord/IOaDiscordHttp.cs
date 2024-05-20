

using System.Diagnostics.CodeAnalysis;
using Discord.WebSocket;

namespace OAHouseChatGpt.Services.Discord;

public interface IOaDiscordHttp
{
    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public Task<DiscordUser> GetUserAsync(string userId);

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public Task<DiscordChannel> GetChannelAsync(string channelId);

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public Task<bool> TriggerTypingAsync(string channelId);

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public Task<DiscordMessage> GetMessageAsync(string channelId, string messageId);

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public Task<bool> SendMessageAsync(string channelId, string content);

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public Task<bool> SendMessageAsync(string channelId, string content, string referenceMessageId);
}

