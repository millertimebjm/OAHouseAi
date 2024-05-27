using OAHouseChatGpt.Models.Usages;

namespace OAHouseChatGpt.Repositories.Usages;

public interface IUsageRepository
{
    Task<UsageModel> GetById(string id);
    Task<string> Upsert(UsageModel model);
    Task<string> Insert(string modelName, string username, int totalTokens);
}