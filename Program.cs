using Autofac;
using OAHouseChatGpt.Services.ChatGpt;
using OAHouseChatGpt.Services.OADiscord;
using OAHouseChatGpt.Services.Configuration;

namespace OAHouseChatGpt
{
    public class Program
    {
        public static async Task Main()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ChatGptService>().As<IChatGpt>();
            builder.RegisterType<OADiscordService>().As<IOADiscord>();
            builder.Register((c, p) =>
            {
                // reasons-why-we-should-ban-kurt
                // https://discord.com/channels/981565185837903927/981756920144756737 
                return new ConfigurationService(
                    0,
                    "",
                    "");
            }).As<IConfiguration>();
            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var discordService = scope.Resolve<IOADiscord>();
                await discordService.Start();
            }

            // See https://aka.ms/new-console-template for more information
            Console.WriteLine("Hello, World!");
        }
    }

}

