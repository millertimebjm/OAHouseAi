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

            await Task.Delay(-1);
        }

        private async Task OnMessageReceived(SocketMessage message)
        {
            Console.WriteLine(message.CleanContent);
            if (message.MentionedUsers.Any(_ => _.Username == _configurationService.GetDiscordBotUsername())
                || message.MentionedRoles.Any(_ => _.Name == _configurationService.GetDiscordBotUsername()))
            {
                var messageWithoutMention =
                    message.CleanContent.Replace(
                        _configurationService.GetDiscordBotUsername(), "");
                Console.WriteLine(messageWithoutMention);
                var textChannel = (await _client.GetChannelAsync(message.Channel.Id)) as SocketTextChannel;
                // var guild = _client.GetGuild(_configurationService.GetOADiscordGuildId());
                // var textChannel = guild.GetTextChannel(_configurationService.GetOADiscordChannelId());
                await textChannel.TriggerTypingAsync();

                if (string.IsNullOrWhiteSpace(message.CleanContent))
                {
                    Thread.Sleep(3000);
                    await textChannel.SendMessageAsync($"{message.Author.Mention} Did you accidentally message the Role instead of the Member?");
                }
                else
                {
                    try
                    {
                        var response = await _gptService.GetTextCompletion(messageWithoutMention);
                        var responseText = response.Choices.FirstOrDefault().Message.Content ?? "";
                        await textChannel.SendMessageAsync($"{message.Author.Mention} {responseText}");
                    }
                    catch
                    {
                        await textChannel.SendMessageAsync($"{message.Author.Mention} There was an error retrieving your response.");
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
                // var channel = await _client.GetChannelAsync(_configurationService.GetOADiscordChannelId());
                // Console.WriteLine(channel.Id);
                // Console.WriteLine(channel.Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}.  {ex.StackTrace}");
            }
        }
    }
}