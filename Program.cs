using Autofac;
using OAHouseChatGpt.Services.ChatGpt;
using OAHouseChatGpt.Services.OADiscord;
using OAHouseChatGpt.Services.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;

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
                // reasons-why-we-should-ban-kurt
                // https://discord.com/channels/1082038301915103253/1082038514285293629
                return new oAHouseChatGptConfigurationService(
                    config.GetConnectionString("DiscordToken"),
                    config.GetConnectionString("OpenAiApiKey"),
                    config.GetConnectionString("DiscordBotUsername"));
            }).As<IOAHouseChatGptConfiguration>();
            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var chatService = scope.Resolve<IChatGpt>();
                var asdf = await chatService.GetTextCompletion("If I AddJsonBody, do I also need to add json header in RestSharp?");

                var discordService = scope.Resolve<IOaDiscord>();
                await discordService.Start();
            }

            // See https://aka.ms/new-console-template for more information
            Console.WriteLine("Hello, World!");
        }
    }
}

