using DataAccess.Abstraction;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Common;

namespace DataAccess;

public class JsonResourceRepository(DbContext context) : IJsonResourceRepository
{
    public async Task<JsonResource?> GetByIdAsync(string id)
    {
        return await context.JsonResources.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<JsonResource>> GetAllAsync()
    {
        return await context.JsonResources.ToListAsync();
    }

    public async Task<IEnumerable<JsonResource>> GetByResourceKindAsync(string resourceKind)
    {
        return await context.JsonResources
            .Where(r => r.ResourceKind == resourceKind)
            .ToListAsync();
    }

    public async Task<IEnumerable<JsonResource>> QueryAsync(Func<IQueryable<JsonResource>, IQueryable<JsonResource>> query)
    {
        return await query(context.JsonResources.AsQueryable()).ToListAsync();
    }

    public async Task<JsonResource> CreateAsync(JsonResource resource)
    {
        context.Entry(resource).Property<DateTime>("CreatedAt").CurrentValue = DateTime.UtcNow;
        context.JsonResources.Add(resource);
        await context.SaveChangesAsync();
        return resource;
    }

    public async Task<Result<JsonResource>> UpdateAsync(JsonResource resource)
    {
        var existing = await context.JsonResources.FirstOrDefaultAsync(r => r.Id == resource.Id);
        if (existing == null)
            return Result<JsonResource>.NotFound("JsonResource", resource.Id);

        context.Entry(existing).Property<DateTime?>("UpdatedAt").CurrentValue = DateTime.UtcNow;
        context.Entry(existing).CurrentValues.SetValues(resource);
        context.Entry(existing).Property(r => r.ResourceKind).IsModified = false;
        await context.SaveChangesAsync();
        return Result<JsonResource>.Ok(existing);
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var resource = await context.JsonResources.FirstOrDefaultAsync(r => r.Id == id);
        if (resource == null)
            return Result.NotFound("JsonResource", id);

        context.Entry(resource).Property<bool>("IsDeleted").CurrentValue = true;
        context.Entry(resource).Property<DateTime?>("UpdatedAt").CurrentValue = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return Result.Ok();
    }
}
