using OaHouseAi.ChatGpt.Services.Interfaces;
using Discord.WebSocket;
using Discord;
using OaHouseAi.Configuration.Services.Interfaces;
using System.Diagnostics.CodeAnalysis;
using OaHouseAi.Repository.Usages.Models;
using System.Text.Json;
using System.Net.WebSockets;
using System.Text;
using OaHouseAi.Discord.Services.Interfaces;
using OaHouseAi.Discord.Models;
using OaHouseAi.ChatGpt.Models;
using OaHouseAi.Repository.Usages.Interfaces;
using Serilog;

namespace OaHouseAi.Discord.Services;

public class OaDiscordService : IOaDiscord
{
    private readonly DiscordSocketClient _client;
    private readonly IChatGpt _gptService;
    private readonly IOAHouseChatGptConfiguration _configurationService;
    private readonly IUsageRepository _usageRepository;
    private readonly IOaDiscordHttp _discordHttpService;
    private static ClientWebSocket _clientWebSocket = new ClientWebSocket();
    private readonly IClientWebSocketWrapper _clientWebSocketWrapper;

    public OaDiscordService(
        IChatGpt gptService,
        IOAHouseChatGptConfiguration configurationService,
        IUsageRepository usageRepository,
        IOaDiscordHttp discordHttpService,
        IClientWebSocketWrapper clientWebSocketWrapper)
    {
        var config = new DiscordSocketConfig() { MessageCacheSize = 100 };
        _client = new DiscordSocketClient(config);
        _gptService = gptService;
        _configurationService = configurationService;
        Log.Debug("OADiscordService: DiscordBotId: {s}", _configurationService.DiscordBotId);
        Log.Debug("OADiscordService: DiscordToken: {s}", _configurationService.OADiscordToken);
        _usageRepository = usageRepository;
        _discordHttpService = discordHttpService;
        _clientWebSocketWrapper = clientWebSocketWrapper;
    }

    [RequiresUnreferencedCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
    [RequiresDynamicCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
    public async Task Start()
    {
        await ConnectToWebSocket();
    }

    [RequiresUnreferencedCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
    [RequiresDynamicCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
    private async Task OnMessageReceived(DiscordMessage message)
    {
        Log.Debug("OADiscordService: Message received from '{s1}' in Channel '{s2}'): {s4}",
            message.Author.Username,
            message.ChannelId,
            message.Content);

        if (!message.MentionedUsers.Any(_ => _.Id == _configurationService.DiscordBotId.ToString()))
            //&& !message.MentionedRoles.Any(_ => _.Id == _discordBotId))
            return;

        var messageWithoutMention = RemoveMentionString(message.Content, _configurationService.DiscordBotId);
        Log.Debug("OADiscordService: received with mention removed: {s1}", messageWithoutMention);

        if (string.IsNullOrWhiteSpace(messageWithoutMention))
        {
            Log.Debug("OADiscordService: Message without mention is either null or whitespace.");
            Thread.Sleep(3000);
            var responseMessage = $"{message.Author.Username} Did you accidentally message the Role instead of the Member?";
            Log.Debug("OADiscordService: Message response sending: {s1}", responseMessage);
            await _discordHttpService.SendMessageAsync(message.ChannelId, responseMessage);
            Log.Debug("OADiscordService: Response sent.");
            return;
        }

        await _discordHttpService.TriggerTypingAsync(message.ChannelId.ToString());
        try
        {
            var context = await GetReferencedMessages(
                message,
                message.ChannelId.ToString(),
                ulong.Parse(_configurationService.DiscordBotId));
            var response = await CallTextCompletionAndWaitWithTyping(
                messageWithoutMention,
                context,
                message.ChannelId.ToString());

            await _usageRepository.Insert(
                response.Model,
                message.Author.Username,
                response.Usage.TotalTokens
            );

            var responseText = response.Choices.FirstOrDefault().Message.Content ?? "";
            await SendLongMessage(
                message.ChannelId.ToString(),
                message.Id.ToString(),
                responseText,
                response.Usage.TotalTokens);
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message.ToString());
            Log.Error(ex.StackTrace.ToString());
            var responseText = $"There was an error retrieving your response.";
            Log.Debug("OADiscordService: Sending message (reference {s1}): {s2}", message.Id, responseText);
            await _discordHttpService.SendMessageAsync(
                message.ChannelId,
                responseText,
                message.Id.ToString());
        }
    }

    [RequiresUnreferencedCode("Calls OAHouseChatGpt.Services.Discord.IOaDiscordHttp.SendMessageAsync(String, String, String)")]
    [RequiresDynamicCode("Calls OAHouseChatGpt.Services.Discord.IOaDiscordHttp.SendMessageAsync(String, String, String)")]
    public async Task SendLongMessage(
        string channelId,
        string referenceMessageId,
        string messageContent,
        int totalTokens,
        int maxMessageLength = 2000)
    {
        int messageCount = (int)Math.Ceiling((double)messageContent.Length / maxMessageLength);

        for (int i = 0; i < messageCount; i++)
        {
            string chunk = messageContent.Substring(i * maxMessageLength, Math.Min(maxMessageLength, messageContent.Length - i * maxMessageLength));
            if (i == messageCount - 1 && totalTokens > 0) chunk += $" (tokens: {totalTokens})";
            Log.Debug("OaDiscordSdkService: SendLongMessage: Just before SendMessageAsync: {s1}\n{s2}\n{s3}", channelId, chunk, referenceMessageId);
            await _discordHttpService.SendMessageAsync(channelId, chunk, referenceMessageId);
        }
    }

    [RequiresUnreferencedCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
    [RequiresDynamicCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
    private async Task<ChatGptResponseModel> CallTextCompletionAndWaitWithTyping(
        string message,
        IEnumerable<ChatGptMessageModel> context,
        string channelId)
    {
        var responseTask = _gptService.GetTextCompletion(message, context);
        while (!responseTask.IsCompleted)
        {
            await _discordHttpService.TriggerTypingAsync(channelId);
            await Task.WhenAny(responseTask, Task.Delay(TimeSpan.FromSeconds(2)));
        }
        return await responseTask;
    }

    [RequiresUnreferencedCode("Calls OAHouseChatGpt.Services.Discord.IOaDiscordHttp.GetMessageAsync(String, String)")]
    [RequiresDynamicCode("Calls OAHouseChatGpt.Services.Discord.IOaDiscordHttp.GetMessageAsync(String, String)")]
    private async Task<IEnumerable<ChatGptMessageModel>> GetReferencedMessages(
        DiscordMessage referenceMessage,
        string channelId,
        ulong discordBotId)
    {
        var context = new List<ChatGptMessageModel>();

        do
        {
            if (referenceMessage.Reference?.IsSpecified ?? false)
            {
                referenceMessage = await _discordHttpService.GetMessageAsync(
                    channelId,
                    referenceMessage.Reference.MessageId);
                context.Add(new ChatGptMessageModel()
                {
                    Role = referenceMessage.Author.Id == discordBotId.ToString() ? "assistant" : "user",
                    Content = RemoveMentionString(RemoveTokenString(referenceMessage.Content), _configurationService.DiscordBotId),
                });
            }
        } while (referenceMessage.Reference?.IsSpecified ?? false);
        context.Reverse();
        return context;
    }

    public static string RemoveTokenString(string s)
    {
        var tokenIndex = s.IndexOf("(tokens: ");
        if (tokenIndex > 0) return s[..(tokenIndex - 1)];
        return s;
    }

    public static string RemoveMentionString(string s, string discordBotId)
    {
        return s.Replace($"<@{discordBotId}>", "")
            .Replace($"\u003C@{discordBotId}\u003E", "")
            .Trim();
    }

    [RequiresUnreferencedCode("Calls OAHouseChatGpt.Services.OADiscord.OaDiscordSdkService.SendHeartbeat()")]
    [RequiresDynamicCode("Calls OAHouseChatGpt.Services.OADiscord.OaDiscordSdkService.SendHeartbeat()")]
    private async Task ConnectToWebSocket()
    {
        string GatewayUrl = "wss://gateway.discord.gg/?v=10&encoding=json";
        await _clientWebSocket.ConnectAsync(new Uri(GatewayUrl), CancellationToken.None);
        Log.Debug("Connected to WebSocket");
        await ReceiveHello();
        await Task.WhenAll(ReceiveMessages(), SendHeartbeat());
    }

    [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.Deserialize<TValue>(String, JsonSerializerOptions)")]
    [RequiresDynamicCode("Calls System.Text.Json.JsonSerializer.Deserialize<TValue>(String, JsonSerializerOptions)")]
    private async Task ReceiveHello()
    {
        var buffer = new byte[1024 * 4];
        var result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        var helloMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
        var helloData = JsonSerializer.Deserialize(helloMessage, DiscordIdentifyJsonSerializerContext.Default.DiscordIdentifyDiscordHeartbeat);
        Console.WriteLine("Hello received: " + helloData);

        await Identify();

        _ = Task.Run(async () =>
        {
            while (true)
            {
                //await Task.Delay(helloData.D.HeartbeatInterval.Value * 1000);
                await Task.Delay(helloData.D.HeartbeatInterval.Value);
                await SendHeartbeat();
            }
        });
    }

    [RequiresUnreferencedCode("Calls OAHouseChatGpt.Services.Discord.DiscordIdentify.Serialize()")]
    [RequiresDynamicCode("Calls OAHouseChatGpt.Services.Discord.DiscordIdentify.Serialize()")]
    public async Task Identify()
    {
        var identifyPayload = new DiscordIdentify<DiscordGatewayIntent>
        {
            Op = 2,
            D = new DiscordGatewayIntent
            {
                Token = _configurationService.OADiscordToken,
                Intents = "3243773",
                Properties = new DiscordGatewayIntentProperties
                {
                    Os = "linux",
                    Browser = "disco",
                    Device = "disco"
                }
            }
        };

        var identifyJson = JsonSerializer.Serialize(identifyPayload, DiscordIdentifyJsonSerializerContext.Default.DiscordIdentifyDiscordGatewayIntent);
        Log.Debug("OaDiscordSdkService: Identify: {s1}", identifyJson);
        var identifyBytes = Encoding.UTF8.GetBytes(identifyJson);
        await _clientWebSocketWrapper.SendAsync(
            _clientWebSocket,
            new ArraySegment<byte>(identifyBytes), 
            WebSocketMessageType.Text, 
            endOfMessage: true, 
            cancellationToken: CancellationToken.None);
        Log.Debug("Identify payload sent");
    }

    [RequiresUnreferencedCode("Calls OAHouseChatGpt.Services.Discord.DiscordIdentify.Serialize()")]
    [RequiresDynamicCode("Calls OAHouseChatGpt.Services.Discord.DiscordIdentify.Serialize()")]
    private async Task SendHeartbeat()
    {
        var heartbeatPayload = new DiscordIdentify<DiscordHeartbeat> { Op = 1, D = null };
        var heartbeatJson = JsonSerializer.Serialize(heartbeatPayload, DiscordIdentifyJsonSerializerContext.Default.DiscordIdentifyDiscordHeartbeat);
        var heartbeatBytes = Encoding.UTF8.GetBytes(heartbeatJson);
        await _clientWebSocket.SendAsync(
            new ArraySegment<byte>(heartbeatBytes), 
            WebSocketMessageType.Text, 
            endOfMessage: true,
            cancellationToken: CancellationToken.None);
        Console.WriteLine("Heartbeat sent");
    }

    [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.Deserialize<TValue>(String, JsonSerializerOptions)")]
    [RequiresDynamicCode("Calls System.Text.Json.JsonSerializer.Deserialize<TValue>(String, JsonSerializerOptions)")]
    private async Task ReceiveMessages()
    {
        var buffer = new byte[1024 * 4];
        var messageBuilder = new StringBuilder(); // Used to accumulate received message

        while (_clientWebSocket.State == WebSocketState.Open)
        {
            var result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                return;
            }
            messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
            if (!result.EndOfMessage) continue;
            var message = messageBuilder.ToString();
            Log.Debug("Received message: {0}", message);
            messageBuilder.Clear();

            var gatewayEvent = JsonSerializer.Deserialize(
                message, DiscordIdentifyBaseJsonSerializerContext.Default.DiscordIdentifyBase);
            Console.WriteLine("Event received: " + gatewayEvent.T);

            if (gatewayEvent.T == "MESSAGE_CREATE")
            {
                var gatewayEventDiscordMessage = JsonSerializer.Deserialize(
                    message, DiscordIdentifyJsonSerializerContext.Default.DiscordIdentifyDiscordMessage);
                Console.WriteLine($"Message received in channel {gatewayEventDiscordMessage.D.ChannelId}/{gatewayEventDiscordMessage.D.Id}: {gatewayEventDiscordMessage.D.Content}");
                await OnMessageReceived(gatewayEventDiscordMessage.D);
            }
        }
    }
}
