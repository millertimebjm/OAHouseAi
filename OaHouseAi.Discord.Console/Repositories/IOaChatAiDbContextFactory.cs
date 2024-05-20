
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace OAHouseChatGpt.Repositories;

public interface IOaHouseAiDbContextFactory
{
    [RequiresDynamicCode("")]
    [RequiresUnreferencedCode("")]
    OaHouseAiDbContext GetDbContext(DbContextTypeEnum type);
}