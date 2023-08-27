using Autofac;
using OAHouseChatGpt.Services.ChatGpt;
using OAHouseChatGpt.Services.OADiscord;
using OAHouseChatGpt.Services.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Discord;
using Serilog;
using Microsoft.Extensions.DependencyInjection;

namespace OAHouseChatGpt
{
    public class Program
    {
        public static async Task Main()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.local.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
            Log.Debug("Logging started.");

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IOAHouseChatGptConfiguration>(c => new oAHouseChatGptConfigurationService(
                config.GetConnectionString("DiscordToken"),
                config.GetConnectionString("OpenAiApiKey"),
                config.GetConnectionString("DiscordBotId")
            ));
            serviceCollection.AddTransient<IChatGpt, ChatGptService>();
            serviceCollection.AddTransient<IOaDiscord, OADiscordService>();
            serviceCollection.AddHttpClient();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var oaDiscordService = serviceProvider.GetRequiredService<IOaDiscord>();
            await oaDiscordService.Start();

            // var chatGptService = serviceProvider.GetRequiredService<IChatGpt>();
            // Console.WriteLine("Send to ChatGpt...");
            // var line = Console.ReadLine();
            // while (!string.IsNullOrWhiteSpace(line))
            // {
            //     var response = await chatGptService.GetTextCompletion(line);
            //     if (response.CompletionStatus == CompletionStatusEnum.Success)
            //     {
            //         Console.WriteLine(response.Choices.First().Message.Content);
            //     }
            //     Console.WriteLine();
            //     line = Console.ReadLine();
            // };
        }
    }
}

