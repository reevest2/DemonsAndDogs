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
        ConfigureResource<SchemaResource>(modelBuilder, "SchemaResources");
        ConfigureResource<PublishedSchemaResource>(modelBuilder, "PublishedSchemaResources");
        ConfigureResource<DataResource>(modelBuilder, "DataResources");
    }
    public DbSet<Resource<JsonResource>> JsonResources { get; set; }
    public DbSet<Resource<SchemaResource>> SchemaResources { get; set; }
    public DbSet<Resource<PublishedSchemaResource>> PublishedSchemaResources { get; set; }
    public DbSet<Resource<DataResource>> DataResources { get; set; }
    
    private static void ConfigureResource<T>(ModelBuilder modelBuilder, string tableName) where T : ResourceBase
    {
        modelBuilder.Entity<Resource<T>>(b =>
        {
            b.ToTable(tableName);

            b.Property(x => x.Data).HasColumnType("jsonb");
        });
    }
}