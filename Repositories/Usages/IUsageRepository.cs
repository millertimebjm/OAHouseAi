using OAHouseChatGpt.Models.Usages;

namespace OAHouseChatGpt.Repositories.Usages;

public interface IUsageRepository 
{
    Task Insert(UsageModel model);
}