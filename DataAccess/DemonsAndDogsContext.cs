using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class DbContext(DbContextOptions<DbContext> options, ResourceRegistry resourceRegistry)
    : IdentityDbContext<IdentityUser, IdentityRole, string>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        resourceRegistry.ApplyModelConfigurations(modelBuilder);
    }
}
