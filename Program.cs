using OAHouseChatGpt.Services.ChatGpt;
using OAHouseChatGpt.Services.OADiscord;
using OAHouseChatGpt.Services.Configuration;
using Microsoft.Extensions.Configuration;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace OAHouseChatGpt
{
    public class Program
    {
        [RequiresUnreferencedCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
        [RequiresDynamicCode("Calls System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync<TValue>(String, TValue, JsonSerializerOptions, CancellationToken)")]
        public static async Task Main()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
            Log.Debug("Logging started.");

            const string _applicationNameConfigurationService = "OaHouseAi";
            const string _appConfigEnvironmentVariableName = "AppConfigConnectionString";

            var builder = new ConfigurationBuilder();
            var config = builder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            string appConfigConnectionString =
                        // Windows config value
                        config[_appConfigEnvironmentVariableName]
                        // Linux config value
                        ?? config[$"Values:{_appConfigEnvironmentVariableName}"]
                        ?? throw new ArgumentNullException(_appConfigEnvironmentVariableName);
            
            config = builder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddAzureAppConfiguration(appConfigConnectionString)
                .Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IOAHouseChatGptConfiguration>(c => new oAHouseChatGptConfigurationService(
                config.GetValue<string>($"{_applicationNameConfigurationService}:DiscordToken"),
                config.GetValue<string>($"{_applicationNameConfigurationService}:OpenAiApiKey"),
                config.GetValue<string>($"{_applicationNameConfigurationService}:DiscordBotId")
            ));
            serviceCollection.AddTransient<IChatGpt, ChatGptService>();
            serviceCollection.AddTransient<IOaDiscord, OADiscordService>();
            serviceCollection.AddHttpClient();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // #region PROD
            // var oaDiscordService = serviceProvider.GetRequiredService<IOaDiscord>();
            // await oaDiscordService.Start();
            // #endregion PROD

            #region For testing purposes...
            
            var chatGptService = serviceProvider.GetRequiredService<IChatGpt>();
            Console.WriteLine("Send to ChatGpt...");
            var line = Console.ReadLine();
            while (!string.IsNullOrWhiteSpace(line))
            {
                var response = await chatGptService.GetTextCompletion(line);
                if (response.CompletionStatus == CompletionStatusEnum.Success)
                {
                    Console.WriteLine(response.Choices.First().Message.Content);
                }
                Console.WriteLine();
                line = Console.ReadLine();
            };
            #endregion
        }
    }
}

