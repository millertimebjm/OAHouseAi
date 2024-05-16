
using Microsoft.EntityFrameworkCore;

namespace OAHouseChatGpt.Repositories;

public interface IOaHouseAiDbContextFactory
{
    OaHouseAiDbContext GetDbContext(string type);
}