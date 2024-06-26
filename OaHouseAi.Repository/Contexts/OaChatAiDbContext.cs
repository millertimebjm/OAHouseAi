using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using OaHouseAi.Repository.Usages.Models;

namespace OaHouseAi.Repository.Contexts;

public class OaHouseAiDbContext : DbContext
{
    public DbSet<UsageModel> Usages { get; init; }

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public OaHouseAiDbContext(DbContextOptions options)
        : base(options)
    {
    }

    [RequiresUnreferencedCode("")]
    [RequiresDynamicCode("")]
    public static OaHouseAiDbContext Create(IMongoDatabase database) =>
        new(new DbContextOptionsBuilder<OaHouseAiDbContext>()
            .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
            .Options);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // base.OnModelCreating(modelBuilder);
        // modelBuilder
        //     .Entity<UsageModel>()
        //     .ToCollection("Usage")
        //     .HasKey(_ => _.Id);
    }
}