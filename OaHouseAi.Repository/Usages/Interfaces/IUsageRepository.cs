using OaHouseAi.Repository.Usages.Models;

namespace OaHouseAi.Repository.Usages.Interfaces;

public interface IUsageRepository
{
    Task<UsageModel> GetById(string id);
    Task<string> Upsert(UsageModel model);
    Task<string> Insert(string modelName, string username, int totalTokens);
}