using OAHouseChatGpt.Services.ChatGpt;
using OAHouseChatGpt.Services.OADiscord;
using OAHouseChatGpt.Services.Configuration;
using Microsoft.Extensions.Configuration;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
//using OAHouseChatGpt.Repositories.Usages;
using OAHouseChatGpt.Repositories;
using OAHouseChatGpt.Repositories.Usages;
using OAHouseChatGpt.Services.Discord;

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
            Log.Debug("Local-only Logging started.");

            const string _applicationNameConfigurationService = "OaHouseAi";
            const string _appConfigEnvironmentVariableName = "AppConfigConnectionString";

            Log.Debug("Looking in this path for local settings: {s1}", Directory.GetCurrentDirectory());

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

            Log.Debug("Program.cs LoggingDbServer: {s}", config.GetValue<string>($"{_applicationNameConfigurationService}:LoggingDbServer"));
            Log.Debug("Program.cs LoggingCollectionName {s}", config.GetValue<string>($"{_applicationNameConfigurationService}:LoggingCollectionName"));

            await Log.CloseAndFlushAsync();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.MongoDB(
                    config.GetValue<string>($"{_applicationNameConfigurationService}:LoggingDbServer"),
                    config.GetValue<string>($"{_applicationNameConfigurationService}:LoggingCollectionName"))
                .CreateLogger();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IOAHouseChatGptConfiguration>(c => new oAHouseChatGptConfigurationService(
                config.GetValue<string>($"{_applicationNameConfigurationService}:DiscordToken"),
                config.GetValue<string>($"{_applicationNameConfigurationService}:OpenAiApiKey"),
                config.GetValue<string>($"{_applicationNameConfigurationService}:DiscordBotId"),
                config.GetValue<string>($"{_applicationNameConfigurationService}:LoggingDbServer"),
                DbContextTypeEnum.MongoDb
            ));
            serviceCollection.AddTransient<IChatGpt, ChatGptService>();
            serviceCollection.AddTransient<IOaDiscordSdk, OaDiscordSdkService>();
            serviceCollection.AddTransient<IOaDiscordHttp, OaDiscordHttpService>();
            serviceCollection.AddTransient<IUsageRepository, MongoDbUsageRepository>();
            serviceCollection.AddTransient<IOaHouseAiDbContextFactory, OaHouseAiDbContextFactoryRollUp>();
            serviceCollection.AddHttpClient();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            #region PROD
            var oaDiscordService = serviceProvider.GetRequiredService<IOaDiscordSdk>();
            await oaDiscordService.Start();
            #endregion PROD

            // #region For testing purposes...
            // var chatGptService = serviceProvider.GetRequiredService<IChatGpt>();
            // Log.Debug("Send to ChatGpt...");
            // Console.WriteLine("Send to ChatGpt...");
            // var line = Console.ReadLine();
            // while (!string.IsNullOrWhiteSpace(line))
            // {
            //     var response = await chatGptService.GetTextCompletion(line);
            //     if (response.CompletionStatus == CompletionStatusEnum.Success)
            //     {
            //         Console.WriteLine(response.Choices.First().Message.Content);
            //     }
            //     Log.Debug("Send to ChatGpt...");
            //     Console.WriteLine("Send to ChatGpt...");
            //     line = Console.ReadLine();
            // };
            // #endregion
        }
    }
}