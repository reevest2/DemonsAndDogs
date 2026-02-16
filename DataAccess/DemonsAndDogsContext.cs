using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Resources;
using Newtonsoft.Json;

namespace DataAccess;

public class DemonsAndDogsContext(DbContextOptions<DemonsAndDogsContext> options)
    : IdentityDbContext<IdentityUser, IdentityRole, string>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    private static void AddResourceConversion<TResource>(ModelBuilder modelBuilder) where TResource : ResourceBase
    {
        modelBuilder
            .Entity<Resource<TResource>>()
            .Property(r => r.Data)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<TResource>(v)!);
    }
}