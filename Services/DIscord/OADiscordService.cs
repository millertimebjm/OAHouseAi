using OAHouseChatGpt.Services.ChatGpt;
using Discord.WebSocket;
using Discord;
using OAHouseChatGpt.Services.Configuration;
using System.Threading;
using System.Text.RegularExpressions;

namespace OAHouseChatGpt.Services.OADiscord
{
    public class OADiscordService : IOaDiscord
    {
        private readonly DiscordSocketClient _client;
        private readonly IChatGpt _gptService;
        private readonly IOAHouseChatGptConfiguration _configurationService;
        private IUser _discordUser;

        public OADiscordService(
            IChatGpt gptService,
            IOAHouseChatGptConfiguration configurationService)
        {
            var config = new DiscordSocketConfig() { MessageCacheSize = 100 };
            _client = new DiscordSocketClient(config);
            _gptService = gptService;
            _configurationService = configurationService;
        }

        public async Task Start()
        {
            _client.MessageReceived += OnMessageReceived;

            _client.Connected += OnConnected;
            await _client.LoginAsync(TokenType.Bot, _configurationService.GetOADiscordToken());
            await _client.StartAsync();
            Console.WriteLine(_client.LoginState);
            Console.WriteLine(_client.ConnectionState);
            var discordBotId = ulong.Parse(_configurationService.GetDiscordBotId());
            _discordUser = await _client.GetUserAsync(discordBotId);
            Console.WriteLine("Ready");
            await Task.Delay(-1);
        }

        private async Task OnMessageReceived(SocketMessage message)
        {
            Console.WriteLine(message.Content);
            var discordBotId = ulong.Parse(_configurationService.GetDiscordBotId());
            if (message.MentionedUsers.Any(_ => _.Id == discordBotId)
                || message.MentionedRoles.Any(_ => _.Id == discordBotId))
            {
                var messageWithoutMention =
                    message.Content.Replace(
                        _discordUser.Mention.Replace("!", ""), "");
                Console.WriteLine(messageWithoutMention);
                var textChannel = (await _client.GetChannelAsync(message.Channel.Id)) as SocketTextChannel;

                if (string.IsNullOrWhiteSpace(messageWithoutMention))
                {
                    Thread.Sleep(3000);
                    await textChannel.SendMessageAsync($"{message.Author.Mention} Did you accidentally message the Role instead of the Member?");
                }
                else
                {
                    try
                    {
                        var context = new List<MessageModel>();
                        var referenceMessage = message as IMessage;
                        do
                        {
                            if (referenceMessage.Reference?.MessageId.IsSpecified ?? false)
                            {
                                referenceMessage = await textChannel.GetMessageAsync(referenceMessage.Reference.MessageId.Value);
                                context.Add(new MessageModel()
                                {
                                    Role = referenceMessage.Author.Id == discordBotId ? "assistant" : "user",
                                    Content = referenceMessage.Content,
                                });
                            }
                        } while (referenceMessage.Reference?.MessageId.IsSpecified ?? false);

                        var responseTask = _gptService.GetTextCompletion(messageWithoutMention, context);
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
                        var responseText = responseTask.Result.Choices.FirstOrDefault().Message.Content ?? "";
                        await textChannel.SendMessageAsync($"{message.Author.Mention} {responseText}",
                            messageReference: new MessageReference(message.Id));
                    }
                    catch (Exception ex)
                    {
                        await textChannel.SendMessageAsync(
                            $"{message.Author.Mention} There was an error retrieving your response.",
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
            }
        }

        private async Task OnConnected()
        {
            try
            {
                Console.WriteLine("Connected");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}.  {ex.StackTrace}");
            }
        }
    }
}