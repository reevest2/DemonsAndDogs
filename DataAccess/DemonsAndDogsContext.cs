using AppConstants;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Common;

namespace DataAccess;

public class DbContext(DbContextOptions<DbContext> options)
    : IdentityDbContext<IdentityUser, IdentityRole, string>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<JsonResource>(b =>
        {
            b.HasKey(r => r.Id);
            b.Property<DateTime>("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
            b.Property<DateTime?>("UpdatedAt");
            b.Property<bool>("IsDeleted").HasDefaultValue(false);
            b.Property<int>("Version").HasDefaultValue(1);

            b.HasDiscriminator(r => r.ResourceKind)
                .HasValue<CampaignResource>(ResourceKinds.Campaign)
                .HasValue<GameResource>(ResourceKinds.Game)
                .HasValue<SchemaResource>(ResourceKinds.Schema)
                .HasValue<DocumentDefinitionResource>(ResourceKinds.DocumentDefinition)
                .HasValue<DocumentResource>(ResourceKinds.Document)
                .HasValue<CharacterResource>(ResourceKinds.Character)
                .HasValue<SessionResource>(ResourceKinds.Session);
            
            b.HasQueryFilter(r => !EF.Property<bool>(r, "IsDeleted"));
        });
    }
    public DbSet<JsonResource> JsonResources { get; set; }
    
}
