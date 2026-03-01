using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Resources;
using Models.Resources.Abstract;

namespace DataAccess;

public class DbContext(DbContextOptions<DbContext> options)
    : IdentityDbContext<IdentityUser, IdentityRole, string>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureResource<JsonResource>(modelBuilder, "JsonResources");
    }
    public DbSet<Resource<JsonResource>> JsonResources { get; set; }
    
    private static void ConfigureResource<T>(ModelBuilder modelBuilder, string tableName) where T : ResourceBase
    {
        modelBuilder.Entity<Resource<T>>(b =>
        {
            b.ToTable(tableName);

            b.Property(x => x.Data).HasColumnType("jsonb");
        });
    }
}