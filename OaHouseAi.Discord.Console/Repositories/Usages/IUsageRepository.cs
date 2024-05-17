using OAHouseChatGpt.Models.Usages;

namespace OAHouseChatGpt.Repositories.Usages;

public interface IUsageRepository 
{
    Task<UsageModel> GetById(string id);
    Task Insert(UsageModel model);
}