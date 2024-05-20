using OAHouseChatGpt.Services.ChatGpt;
using Discord.WebSocket;
using Discord;
using OAHouseChatGpt.Services.Configuration;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using OAHouseChatGpt.Models.Usages;
using OAHouseChatGpt.Repositories.Usages;
using System.Text.Json;
using OAHouseChatGpt.Services.Discord;

namespace OAHouseChatGpt.Services.OADiscord
{
    public class OaDiscordSdkService : IOaDiscordSdk
    {
        private readonly DiscordSocketClient _client;
        private readonly IChatGpt _gptService;
        private readonly IOAHouseChatGptConfiguration _configurationService;
        private readonly ulong _discordBotId;
        private readonly string _discordToken;
        private readonly IUsageRepository _usageRepository;
        private readonly IOaDiscordHttp _discordHttpService;

        public OaDiscordSdkService(
            IChatGpt gptService,
            IOAHouseChatGptConfiguration configurationService,
            IUsageRepository usageRepository,
            IOaDiscordHttp discordHttpService)
        {
            var config = new DiscordSocketConfig() { MessageCacheSize = 100 };
            _client = new DiscordSocketClient(config);
            _gptService = gptService;
            _configurationService = configurationService;
            Log.Debug("OADiscordService: DiscordBotId: {s}", _configurationService.DiscordBotId);
            _discordBotId = ulong.Parse(_configurationService.DiscordBotId);
            Log.Debug("OADiscordService: DiscordToken: {s}", _configurationService.OADiscordToken);
            _discordToken = _configurationService.OADiscordToken;
            _usageRepository = usageRepository;
            _discordHttpService = discordHttpService;
        }

        [RequiresUnreferencedCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
        [RequiresDynamicCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
        public async Task Start()
        {
            _client.MessageReceived += OnMessageReceived;

            _client.Connected += OnConnected;
            await _client.LoginAsync(TokenType.Bot, _discordToken);
            await _client.StartAsync();
            Log.Debug($"OADiscordService: {_client.LoginState}");
            Log.Debug($"OADiscordService: {_client.ConnectionState}");
            Log.Debug("OADiscordService: Ready");
            await Task.Delay(-1);
        }

        [RequiresUnreferencedCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
        [RequiresDynamicCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
        private async Task OnMessageReceived(SocketMessage message)
        {
            // Log.Debug("OaDiscordService: OnMessageReceived: just before serialize");
            // var user = await _discordHttpService.GetUserAsync(message.Author.Id.ToString());
            // Log.Debug("OaDiscordService: OnMessageReceived: {s}", user.Serialize());

            Log.Debug($"OADiscordService: Message received from '{message.Author.Username}' in Channel '{message.Channel.Name}' ({message.Channel.Id}): {message.Content}");

            if (!message.MentionedUsers.Any(_ => _.Id == _discordBotId)
                && !message.MentionedRoles.Any(_ => _.Id == _discordBotId))
                return;
                
            var messageWithoutMention =
                message.Content.Replace(
                    $"<@{_configurationService.DiscordBotId}>", "")
                    .Trim();
            
            Log.Debug($"OADiscordService: received with mention removed: {messageWithoutMention}");
            
            var textChannel = _client.GetChannelAsync(message.Channel.Id);

            if (string.IsNullOrWhiteSpace(messageWithoutMention))
            {
                Log.Debug("OADiscordService: Message without mention is either null or whitespace.");
                Thread.Sleep(3000);
                var responseMessage = $"{message.Author.Mention} Did you accidentally message the Role instead of the Member?";
                Log.Debug($"OADiscordService: Message response sending: {responseMessage}");
                await (await textChannel as SocketTextChannel).SendMessageAsync();
                Log.Debug($"OADiscordService: Response sent.");
                return;
            }

            await _discordHttpService.TriggerTypingAsync(message.Channel.Id.ToString());
            try
            {
                var context = await GetReferencedMessages(
                    message,
                    await textChannel as SocketTextChannel,
                    _discordBotId);
                var response = await CallTextCompletionAndWaitWithTyping(
                    messageWithoutMention,
                    context,
                    await textChannel as SocketTextChannel);

                var usage = new UsageModel()
                {
                    ModelName = response.Model,
                    Username = message.Author.Username,
                    TotalTokens = response.Usage.TotalTokens,
                    UtcTimestamp = DateTime.UtcNow
                };
                await _usageRepository.Insert(usage);

                Log.Debug(
                    "OADiscordSdkService: Usage report filed: {s}",
                    usage.Serialize());

                var responseText = response.Choices.FirstOrDefault().Message.Content ?? "";
                await SendLongMessage(
                    //await textChannel as SocketTextChannel, 
                    message.Channel.Id.ToString(),
                    //new MessageReference(),
                    message.Id.ToString(),
                    responseText, 
                    response.Usage.TotalTokens);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message.ToString());
                Log.Error(ex.StackTrace.ToString());
                var responseText = $"There was an error retrieving your response.";
                Log.Debug($"OADiscordService: Sending message (reference ${message.Id}): ${responseText}");
                await (await textChannel as SocketTextChannel).SendMessageAsync(
                    responseText,
                    messageReference: new MessageReference(message.Id));
            }
        }

        [RequiresUnreferencedCode("Calls OAHouseChatGpt.Services.Discord.IOaDiscordHttp.SendMessageAsync(String, String, String)")]
        [RequiresDynamicCode("Calls OAHouseChatGpt.Services.Discord.IOaDiscordHttp.SendMessageAsync(String, String, String)")]
        private async Task SendLongMessage(
            //SocketTextChannel textChannel,
            string channelId,
            //MessageReference messageReference,
            string referenceMessageId,
            string messageContent, 
            int totalTokens)
        {
            const int MaxMessageLength = 2000;
            int messageCount = (int)Math.Ceiling((double)messageContent.Length / MaxMessageLength);

            for (int i = 0; i < messageCount; i++)
            {
                string chunk = messageContent.Substring(i * MaxMessageLength, Math.Min(MaxMessageLength, messageContent.Length - i * MaxMessageLength));
                if (i == messageCount -1 && totalTokens > 0) chunk += $" (tokens: {totalTokens})";
                //await textChannel.SendMessageAsync(chunk, messageReference: messageReference);
                Log.Debug("OaDiscordSdkService: SendLongMessage: Just before SendMessageAsync: {s1}\n{s2}\n{s3}", channelId, chunk, referenceMessageId);
                await _discordHttpService.SendMessageAsync(channelId, chunk, referenceMessageId);
            }
        }

        [RequiresUnreferencedCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
        [RequiresDynamicCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
        private async Task<ChatGptResponseModel> CallTextCompletionAndWaitWithTyping(
            string message,
            IEnumerable<ChatGptMessageModel> context,
            SocketTextChannel textChannel)
        {
            var responseTask = _gptService.GetTextCompletion(message, context);
            while (!responseTask.IsCompleted)
            {
                await textChannel.TriggerTypingAsync();
                await Task.WhenAny(responseTask, Task.Delay(TimeSpan.FromSeconds(2)));
            }
            return await responseTask;
        }

        // private async Task<IEnumerable<ChatGptMessageModel>> GetReferencedMessages(
        //     IMessage message,
        //     SocketTextChannel textChannel,
        //     ulong discordBotId)
        // {
        //     var context = new List<ChatGptMessageModel>();
        //     var referenceMessage = message;
        //     do
        //     {
        //         if (referenceMessage.Reference?.MessageId.IsSpecified ?? false)
        //         {
        //             referenceMessage = await textChannel.GetMessageAsync(referenceMessage.Reference.MessageId.Value);
        //             context.Add(new ChatGptMessageModel()
        //             {
        //                 Role = referenceMessage.Author.Id == discordBotId ? "assistant" : "user",
        //                 Content = referenceMessage.Content,
        //             });
        //         }
        //     } while (referenceMessage.Reference?.MessageId.IsSpecified ?? false);
        //     context.Reverse();
        //     return context;
        // }

        [RequiresUnreferencedCode("Calls OAHouseChatGpt.Services.OADiscord.OaDiscordSdkService.GetReferencedMessages(DiscordMessage, SocketTextChannel, UInt64)")]
        [RequiresDynamicCode("Calls OAHouseChatGpt.Services.OADiscord.OaDiscordSdkService.GetReferencedMessages(DiscordMessage, SocketTextChannel, UInt64)")]
        private async Task<IEnumerable<ChatGptMessageModel>> GetReferencedMessages(
            IMessage message,
            SocketTextChannel textChannel,
            ulong discordBotId)
        {
            DiscordMessage referenceMessage = new DiscordMessage()
            {
                Id = message.Id.ToString(),
                ChannelId = message.Channel.Id.ToString(),
                Timestamp = message.CreatedAt.UtcDateTime,
                Author = new DiscordUser() 
                {
                    Id = message.Author.Id.ToString(),
                },
                Reference = new DiscordMessageReference()
                {
                    MessageId = message.Reference?.MessageId.Value.ToString(),
                    ChannelId = message.Reference?.ChannelId.ToString(),
                    GuildId = message.Reference?.GuildId.Value.ToString(),
                },
            };
            return await GetReferencedMessages(referenceMessage,
                textChannel,
                discordBotId);       
        }

        [RequiresUnreferencedCode("Calls OAHouseChatGpt.Services.Discord.IOaDiscordHttp.GetMessageAsync(String, String)")]
        [RequiresDynamicCode("Calls OAHouseChatGpt.Services.Discord.IOaDiscordHttp.GetMessageAsync(String, String)")]
        private async Task<IEnumerable<ChatGptMessageModel>> GetReferencedMessages(
            DiscordMessage referenceMessage,
            SocketTextChannel textChannel,
            ulong discordBotId)
        {
            var context = new List<ChatGptMessageModel>();
            
            do
            {
                if (referenceMessage.Reference?.IsSpecified ?? false)
                {
                    referenceMessage = await _discordHttpService.GetMessageAsync(textChannel.Id.ToString(), referenceMessage.Reference.MessageId);
                    context.Add(new ChatGptMessageModel()
                    {
                        Role = referenceMessage.Author.Id == discordBotId.ToString() ? "assistant" : "user",
                        Content = referenceMessage.Content,
                    });
                }
            } while (referenceMessage.Reference?.IsSpecified ?? false);
            context.Reverse();
            return context;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task OnConnected()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            try
            {
                Log.Debug("OADiscordService: Connected");
            }
            catch (Exception ex)
            {
                Log.Debug($"OADiscordService: {ex.Message}. {ex.StackTrace}");
            }
        }
    }
}