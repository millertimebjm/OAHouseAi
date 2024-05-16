using OAHouseChatGpt.Services.ChatGpt;
using Discord.WebSocket;
using Discord;
using OAHouseChatGpt.Services.Configuration;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using OAHouseChatGpt.Models.Usages;
using OAHouseChatGpt.Repositories.Usages;
using System.Text.Json;

namespace OAHouseChatGpt.Services.OADiscord
{
    public class OADiscordService : IOaDiscord
    {
        private readonly DiscordSocketClient _client;
        private readonly IChatGpt _gptService;
        private readonly IOAHouseChatGptConfiguration _configurationService;
        private IUser _discordUser;
        private readonly ulong _discordBotId;
        private readonly string _discordToken;
        private readonly IUsageRepository _usageRepository;

        public OADiscordService(
            IChatGpt gptService,
            IOAHouseChatGptConfiguration configurationService,
            IUsageRepository usageRepository)
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
            _discordUser = await _client.GetUserAsync(_discordBotId);
            Log.Debug("OADiscordService: Ready");
            await Task.Delay(-1);
        }

        [RequiresUnreferencedCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
        [RequiresDynamicCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
        private async Task OnMessageReceived(SocketMessage message)
        {
            Log.Debug($"OADiscordService: Message received from '{message.Author.Username}': {message.Content}");
            if (!message.MentionedUsers.Any(_ => _.Id == _discordBotId)
                && !message.MentionedRoles.Any(_ => _.Id == _discordBotId))
                return;

            var messageWithoutMention =
                message.Content.Replace(
                    _discordUser.Mention.Replace("!", ""), "");
            Log.Debug($"OADiscordService: received with mention removed: {messageWithoutMention}");
            var textChannel = (await _client.GetChannelAsync(message.Channel.Id)) as SocketTextChannel;

            if (string.IsNullOrWhiteSpace(messageWithoutMention))
            {
                Log.Debug("OADiscordService: Message without mention is either null or whitespace.");
                Thread.Sleep(3000);
                var responseMessage = $"{message.Author.Mention} Did you accidentally message the Role instead of the Member?";
                Log.Debug($"OADiscordService: Message response sending: {responseMessage}");
                await textChannel.SendMessageAsync();
                Log.Debug($"OADiscordService: Response sent.");
                return;
            }

            try
            {
                var context = await GetReferencedMessages(
                    message,
                    textChannel,
                    _discordBotId);
                var response = await CallTextCompletionAndWaitWithTyping(
                    messageWithoutMention,
                    context,
                    textChannel);

                var usage = new UsageModel()
                {
                    ModelName = response.Model,
                    Username = message.Author.Username,
                    TotalTokens = response.Usage.TotalTokens,
                    UtcTimestamp = DateTime.UtcNow
                };
                await _usageRepository.Insert(usage);

                Log.Debug(
                    "OADiscordService: Usage report filed: {s}",
                    JsonSerializer.Serialize(usage, new JsonSerializerOptions()
                    {
                        TypeInfoResolver = new UsageModelJsonSerializerContext(),
                    }));

                var responseText = response.Choices.FirstOrDefault().Message.Content ?? "";
                await SendLongMessage(textChannel, new MessageReference(message.Id), responseText);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message.ToString());
                Log.Error(ex.StackTrace.ToString());
                var responseText = $"There was an error retrieving your response.";
                Log.Debug($"OADiscordService: Sending message (reference ${message.Id}): ${responseText}");
                await textChannel.SendMessageAsync(
                    responseText,
                    messageReference: new MessageReference(message.Id));
            }
        }

        private async Task SendLongMessage(
            SocketTextChannel textChannel,
            MessageReference messageReference,
            string messageContent)
        {
            const int MaxMessageLength = 2000;
            int messageCount = (int)Math.Ceiling((double)messageContent.Length / MaxMessageLength);

            for (int i = 0; i < messageCount; i++)
            {
                string chunk = messageContent.Substring(i * MaxMessageLength, Math.Min(MaxMessageLength, messageContent.Length - i * MaxMessageLength));
                await textChannel.SendMessageAsync(chunk, messageReference: messageReference);
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

        private async Task<IEnumerable<ChatGptMessageModel>> GetReferencedMessages(
            IMessage message,
            SocketTextChannel textChannel,
            ulong discordBotId)
        {
            var context = new List<ChatGptMessageModel>();
            var referenceMessage = message as IMessage;
            do
            {
                if (referenceMessage.Reference?.MessageId.IsSpecified ?? false)
                {
                    referenceMessage = await textChannel.GetMessageAsync(referenceMessage.Reference.MessageId.Value);
                    context.Add(new ChatGptMessageModel()
                    {
                        Role = referenceMessage.Author.Id == discordBotId ? "assistant" : "user",
                        Content = referenceMessage.Content,
                    });
                }
            } while (referenceMessage.Reference?.MessageId.IsSpecified ?? false);
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