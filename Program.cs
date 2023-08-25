using Autofac;
using OAHouseChatGpt.Services.ChatGpt;
using OAHouseChatGpt.Services.OADiscord;
using OAHouseChatGpt.Services.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;

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

            var builder = new ContainerBuilder();
            builder.RegisterType<ChatGptService>().As<IChatGpt>();
            builder.RegisterType<OADiscordService>().As<IOaDiscord>();
            builder.Register((c, p) =>
            {
                return new oAHouseChatGptConfigurationService(
                    config.GetConnectionString("DiscordToken"),
                    config.GetConnectionString("OpenAiApiKey"),
                    config.GetConnectionString("DiscordBotId"));
            }).As<IOAHouseChatGptConfiguration>();
            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                // var chatGptService = scope.Resolve<IChatGpt>();
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


                var discordService = scope.Resolve<IOaDiscord>();
                await discordService.Start();
            }
        }
    }
}

