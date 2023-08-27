using OAHouseChatGpt.Services.ChatGpt;
using Discord.WebSocket;
using Discord;
using OAHouseChatGpt.Services.Configuration;
using Serilog;

namespace OAHouseChatGpt.Services.OADiscord
{
    public class OADiscordService : IOaDiscord
    {
        private readonly DiscordSocketClient _client;
        private readonly IChatGpt _gptService;
        private readonly IOAHouseChatGptConfiguration _configurationService;
        private IUser _discordUser;
        private readonly ulong _discordBotId;

        public OADiscordService(
            IChatGpt gptService,
            IOAHouseChatGptConfiguration configurationService)
        {
            var config = new DiscordSocketConfig() { MessageCacheSize = 100 };
            _client = new DiscordSocketClient(config);
            _gptService = gptService;
            _configurationService = configurationService;
            _discordBotId = ulong.Parse(_configurationService.GetDiscordBotId());
        }

        public async Task Start()
        {
            _client.MessageReceived += OnMessageReceived;

            _client.Connected += OnConnected;
            await _client.LoginAsync(TokenType.Bot, _configurationService.GetOADiscordToken());
            await _client.StartAsync();
            Log.Debug($"OADiscordService: {_client.LoginState}");
            Log.Debug($"OADiscordService: {_client.ConnectionState.ToString()}");
            _discordUser = await _client.GetUserAsync(_discordBotId);
            Log.Debug("OADiscordService: Ready");
            await Task.Delay(-1);
        }

        private async Task OnMessageReceived(SocketMessage message)
        {
            Log.Debug($"OADiscordService: Message received: {message.Content}");
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
                var responseText = response.Choices.FirstOrDefault().Message.Content ?? "";
                await textChannel.SendMessageAsync($"{responseText}",
                    messageReference: new MessageReference(message.Id));
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
            // if (response.CompletionStatus == CompletionStatusEnum.Success)
            // {
            //     await textChannel.SendMessageAsync($"{message.Author.Mention} {responseText}");
            // }
            // else
            // {
            //     await textChannel.SendMessageAsync($"{message.Author.Mention} There was an error retrieving your response.");
            // }
        }

        private async Task<ChatGptResponseModel> CallTextCompletionAndWaitWithTyping(
            string message,
            IEnumerable<ChatGptMessageModel> context,
            SocketTextChannel textChannel)
        {
            var responseTask = _gptService.GetTextCompletion(message, context);
            //#pragma warning disable RCS4014, cs4014
            Task.Run(async () =>
            {
                await responseTask;
            });
            while (!responseTask.IsCompleted)
            {
                await textChannel.TriggerTypingAsync();
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
            return responseTask.Result;
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

        private async Task OnConnected()
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