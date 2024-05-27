
// using System.Diagnostics.CodeAnalysis;
// using System.Net.Http.Json;
// using OAHouseChatGpt.Models.Usages;
// using OAHouseChatGpt.Services.Configuration;
// using Serilog;

// namespace OAHouseChatGpt.Repositories.Usages;

// public class HttpUsageRepository : IUsageRepository
// {
//     private readonly IHttpClientFactory _httpClientFactory;
//     private readonly IOAHouseChatGptConfiguration _config;
//     private const string _collectionName = "Usage";
//     public HttpUsageRepository(
//         IHttpClientFactory httpClientFactory,
//         IOAHouseChatGptConfiguration config)
//     {
//         _httpClientFactory = httpClientFactory;
//         _config = config;
//     }

//     [RequiresUnreferencedCode("")]
//     [RequiresDynamicCode("")]
//     public async Task<UsageModel> GetById(string id)
//     {
//         var httpClient = _httpClientFactory.CreateClient();
//         var response = await httpClient.GetAsync(
//             $"{_config.DatabaseConnectionString}/databases/{_config.DatabaseName}/collections/{_collectionName}/{id}");
//         if (response.IsSuccessStatusCode)
//         {
//             return await response.Content.ReadFromJsonAsync<UsageModel>(
//                 UsageModel.GetJsonSerializerOptions());
//         }
//         Log.Debug(
//             "HttpUsageRepository: GetById: Id was not found: {s1}/{s2}/{s3}", 
//             _config.DatabaseConnectionString, 
//             _collectionName,
//             id);
//         return null;
//     }

//     [RequiresUnreferencedCode("")]
//     [RequiresDynamicCode("")]
//     public async Task<UsageModel> Insert(UsageModel model)
//     {
//         var httpClient = _httpClientFactory.CreateClient();
//         var mongoConnectionString = $"{_config.DatabaseServer}/databases/{_config.DatabaseName}/collections/{_collectionName}";
//         var response = await httpClient.PostAsJsonAsync(
//             mongoConnectionString,
//             model, 
//             UsageModel.GetJsonSerializerOptions());
//         if (response.IsSuccessStatusCode)
//         {
//             Log.Debug(
//                 "HttpUsageRepository: Insert: Usage inserted: {s1}/{s2}\n{s3}", 
//                 _config.DatabaseConnectionString,
//                 _config.DatabaseName,
//                 _collectionName,
//                 model.Serialize());
//             return await response.Content.ReadFromJsonAsync<UsageModel>(
//                 UsageModel.GetJsonSerializerOptions());
//         }
//         Log.Debug(
//             "HttpUsageRepository: Insert: Usage insert failed: {s1}/{s2}\n{s3}", 
//             _config.DatabaseConnectionString,
//             _config.DatabaseName,
//             _collectionName,
//             model.Serialize());
//         return null;
//     }

//     [RequiresUnreferencedCode("")]
//     [RequiresDynamicCode("")]
//     public async Task<UsageModel> Insert(string modelName, string username, int totalTokens)
//     {
//         return await Insert(new UsageModel() 
//         {
//             ModelName = modelName,
//             Username = username,
//             TotalTokens = totalTokens,
//             UtcTimestamp = DateTime.UtcNow,
//         });
//     }
// }
