using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Resources;
using Newtonsoft.Json;

namespace DataAccess;

public class DemonsAndDogsContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public DemonsAndDogsContext(DbContextOptions<DemonsAndDogsContext> options) : base(options) { }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        void AddResourceConversion<TResource>() where TResource : ResourceBase
        {
            modelBuilder
                .Entity<Resource<TResource>>()
                .Property(r => r.Data)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<TResource>(v));
        }
    }
}