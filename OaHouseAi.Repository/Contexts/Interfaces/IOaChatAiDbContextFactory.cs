using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace OaHouseAi.Repository.Contexts.Interfaces;

public interface IOaHouseAiDbContextFactory
{
    [RequiresDynamicCode("")]
    [RequiresUnreferencedCode("")]
    OaHouseAiDbContext GetDbContext(DbContextTypeEnum type);
}