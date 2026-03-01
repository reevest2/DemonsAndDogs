using System.Linq.Expressions;
using DataAccess.Abstraction;
using Microsoft.EntityFrameworkCore;
using Models.Resources;
using Models.Resources.Abstract;

namespace DataAccess;

public class ResourceRepository<TResource> : IResourceRepository<TResource> where TResource : ResourceBase
{
    protected readonly DbContext _context;

    public ResourceRepository(DbContext context)
    {
        _context = context;
    }

    public DbSet<TResource> GetDbSet() => _context.Set<TResource>();

    public IQueryable<Resource<TResource>> GetQuery() =>
        _context.Set<Resource<TResource>>().AsNoTracking().AsQueryable();

    public virtual async Task<List<TResource>> GetAllAsync(params Expression<Func<Resource<TResource>, bool>>[] filters)
    {
        IQueryable<Resource<TResource>> query = _context.Set<Resource<TResource>>()
            .AsNoTracking()
            .Where(r => !r.IsDeleted);

        if (filters is { Length: > 0 })
            query = filters.Aggregate(query, (current, filter) => current.Where(filter));

        return await query.Select(r => r.Data).ToListAsync();
    }

    public virtual async Task<TResource> GetByIdAsync(string id)
    {
        var resource = await _context.Set<Resource<TResource>>()
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        if (resource == null)
            throw new KeyNotFoundException(id);

        return resource.Data;
    }

    public virtual async Task<TResource> GetByOwnerAsync(string ownerId, params Expression<Func<Resource<TResource>, bool>>[] filters)
    {
        IQueryable<Resource<TResource>> query = _context.Set<Resource<TResource>>()
            .AsNoTracking()
            .Where(r => r.OwnerId == ownerId && !r.IsDeleted);

        if (filters is { Length: > 0 })
            query = filters.Aggregate(query, (current, filter) => current.Where(filter));

        var resource = await query.FirstOrDefaultAsync();

        if (resource == null)
            throw new KeyNotFoundException(ownerId);

        return resource.Data;
    }

    public virtual async Task<TResource> FirstOrDefaultAsync(string ownerId, params Expression<Func<Resource<TResource>, bool>>[] filters)
    {
        IQueryable<Resource<TResource>> query = _context.Set<Resource<TResource>>()
            .AsNoTracking()
            .Where(r => r.OwnerId == ownerId && !r.IsDeleted);

        if (filters is { Length: > 0 })
            query = filters.Aggregate(query, (current, filter) => current.Where(filter));

        var resource = await query.FirstOrDefaultAsync();

        if (resource == null)
            return default!;

        return resource.Data;
    }

    public virtual async Task<List<TResource>> GetListByOwnerAsync(string ownerId)
    {
        return await _context.Set<Resource<TResource>>()
            .AsNoTracking()
            .Where(r => r.OwnerId == ownerId && !r.IsDeleted)
            .OrderByDescending(r => r.UpdatedAt)
            .Select(r => r.Data)
            .ToListAsync();
    }

    public virtual async Task<int> GetCountByOwnerAsync(string ownerId, bool includeDeleted = false)
    {
        IQueryable<Resource<TResource>> query = _context.Set<Resource<TResource>>()
            .AsNoTracking()
            .Where(r => r.OwnerId == ownerId);

        if (!includeDeleted)
            query = query.Where(r => !r.IsDeleted);

        return await query.CountAsync();
    }

    public virtual async Task<List<TResource>> GetBySubjectId(string subjectId)
    {
        return await _context.Set<Resource<TResource>>()
            .AsNoTracking()
            .Where(r => r.SubjectId == subjectId && !r.IsDeleted)
            .Select(r => r.Data)
            .ToListAsync();
    }

    public virtual async Task<TResource> GetByOwnerIdAndSubjectId(string ownerId, string subjectId)
    {
        var resource = await _context.Set<Resource<TResource>>()
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.OwnerId == ownerId && r.SubjectId == subjectId && !r.IsDeleted);

        if (resource == null)
            throw new KeyNotFoundException($"{ownerId}:{subjectId}");

        return resource.Data;
    }

    public virtual async Task<TResource> CreateResourceAsync(TResource data)
    {
        var timestamp = DateTime.UtcNow;

        var resource = new Resource<TResource>
        {
            Id = data.Id ?? Guid.NewGuid().ToString(),
            OwnerId = data.OwnerId,
            SubjectId = data.SubjectId,
            EntityId = data.EntityId,
            CampaignId = data.CampaignId,
            RulesetId = data.RulesetId,
            GameId = data.GameId,
            SchemaVersion = data.SchemaVersion,
            ResourceKind = data.ResourceKind,
            CreatedAt = timestamp,
            UpdatedAt = timestamp,
            Version = 1,
            IsDeleted = false,
            Data = data
        };

        _context.Set<Resource<TResource>>().Add(resource);
        await _context.SaveChangesAsync();

        return resource.Data;
    }

    public virtual async Task<TResource> UpdateResourceAsync(TResource data)
    {
        var resource = await _context.Set<Resource<TResource>>()
            .FirstOrDefaultAsync(r => r.Id == data.Id);

        if (resource == null)
            throw new KeyNotFoundException(data.Id);

        resource.UpdatedAt = DateTime.UtcNow;
        resource.OwnerId = data.OwnerId;
        resource.SubjectId = data.SubjectId;
        resource.EntityId = data.EntityId;
        resource.CampaignId = data.CampaignId;
        resource.RulesetId = data.RulesetId;
        resource.GameId = data.GameId;
        resource.SchemaVersion = data.SchemaVersion;
        resource.ResourceKind = data.ResourceKind;
        resource.Version++;
        resource.Data = data;

        await _context.SaveChangesAsync();
        return data;
    }

    public virtual async Task DeleteResourceAsync(TResource data)
    {
        var resource = await _context.Set<Resource<TResource>>()
            .FirstOrDefaultAsync(r => r.Id == data.Id);

        if (resource != null)
        {
            _context.Set<Resource<TResource>>().Remove(resource);
            await _context.SaveChangesAsync();
        }
    }
}