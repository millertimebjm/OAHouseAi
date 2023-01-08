using OAHouseChatGpt.Services.ChatGpt;
using Discord.WebSocket;
using Discord;
using OAHouseChatGpt.Services.Configuration;
using System.Threading;

namespace OAHouseChatGpt.Services.OADiscord
{
    public class OADiscordService : IOADiscord
    {
        private readonly DiscordSocketClient _client;
        private readonly IChatGpt _gptService;
        private readonly IConfiguration _configurationService;

        public OADiscordService(
            IChatGpt gptService,
            IConfiguration configurationService)
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
            Console.WriteLine("Got Here");
            Console.WriteLine(message.Content);
            if (message.Channel.Id == _configurationService.GetOADiscordChannelId())
            {
                Console.WriteLine(message.Content);
            }
        }

        private async Task OnConnected()
        {
            try
            {
                Console.WriteLine("Connected");
                var channel = await _client.GetChannelAsync(_configurationService.GetOADiscordChannelId());
                Console.WriteLine(channel.Id);
                Console.WriteLine(channel.Name);
                var userList = await channel.GetUsersAsync().ToListAsync();
                Console.WriteLine(userList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}.  {ex.StackTrace}");
            }
        }
    }
}