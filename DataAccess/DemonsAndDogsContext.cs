using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Character;
using Models.Resources;
using Newtonsoft.Json;

namespace DataAccess;

public class DbContext(DbContextOptions<DbContext> options)
    : IdentityDbContext<IdentityUser, IdentityRole, string>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureResource<TestResource>(modelBuilder, AppConstants.ResourceTableNames.TestResources);
        ConfigureResource<CharacterResource>(modelBuilder, AppConstants.ResourceTableNames.CharacterResources);
        ConfigureResource<RulesetResource>(modelBuilder, AppConstants.ResourceTableNames.RulesetResources);
    }
    
    public DbSet<Resource<TestResource>> TestResources { get; set; }
    public DbSet<Resource<CharacterResource>> CharacterResources { get; set; }
    public DbSet<Resource<RulesetResource>> RulesetResources { get; set; }

    private static void ConfigureResource<T>(ModelBuilder modelBuilder, string tableName)
    {
        modelBuilder.Entity<Resource<T>>(b =>
        {
            b.ToTable(tableName);
            b.Property(x => x.Data).HasColumnType("jsonb");
        });
    }
}