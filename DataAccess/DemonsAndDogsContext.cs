using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Character;
using Models.Resources;
using Models.Resources.Abstract;
using Newtonsoft.Json;

namespace DataAccess;

public class DbContext(DbContextOptions<DbContext> options)
    : IdentityDbContext<IdentityUser, IdentityRole, string>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureResource<TestResource>(modelBuilder, AppConstants.ResourceKeys.TestResources);
        ConfigureResource<CharacterData>(modelBuilder, AppConstants.ResourceKeys.CharacterResources);
        ConfigureResource<RulesetResource>(modelBuilder, AppConstants.ResourceKeys.RulesetResources);
        ConfigureResource<CharacterTemplateData>(modelBuilder, AppConstants.ResourceKeys.CharacterTemplateResources);
    }
    
    public DbSet<Resource<TestResource>> TestResources { get; set; }
    public DbSet<Resource<CharacterData>> CharacterResources { get; set; }
    public DbSet<Resource<RulesetResource>> RulesetResources { get; set; }
    public DbSet<Resource<CharacterTemplateData>> CharacterTemplateResources { get; set; }

    private static void ConfigureResource<T>(ModelBuilder modelBuilder, string tableName)
    {
        modelBuilder.Entity<Resource<T>>(b =>
        {
            b.ToTable(tableName);
            b.Property(x => x.Data).HasColumnType("jsonb");
        });
    }
}