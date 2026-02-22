using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Resources;
using Models.Resources.Abstract;
using Models.Resources.Character;
using Models.Resources.Ruleset;
using Newtonsoft.Json;

namespace DataAccess;

public class DbContext(DbContextOptions<DbContext> options)
    : IdentityDbContext<IdentityUser, IdentityRole, string>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureResource<CampaignData>(modelBuilder, AppConstants.ResourceKeys.CampaignResources);
        ConfigureResource<RulesetResource>(modelBuilder, AppConstants.ResourceKeys.RulesetResources);
        ConfigureResource<TemplateData>(modelBuilder, AppConstants.ResourceKeys.TemplateResources);
        ConfigureResource<EntityData>(modelBuilder, AppConstants.ResourceKeys.EntityResources);
    }
    
    public DbSet<Resource<CampaignData>> CampaignResources { get; set; }
    public DbSet<Resource<RulesetResource>> RulesetResources { get; set; }
    public DbSet<Resource<TemplateData>> TemplateResources { get; set; }
    public DbSet<Resource<EntityData>> EntityResources { get; set; }

    private static void ConfigureResource<T>(ModelBuilder modelBuilder, string tableName)
    {
        modelBuilder.Entity<Resource<T>>(b =>
        {
            b.ToTable(tableName);
            b.Property(x => x.Data).HasColumnType("jsonb");
        });
    }
}