using System.Linq.Expressions;
using DataAccess.Abstraction;
using Microsoft.EntityFrameworkCore;
using Models.Resources;
using Models.Resources.Abstract;

namespace DataAccess;

public class ResourceRepository<TResource> : IResourceRepository.IResourceRepository<TResource>
{
    protected readonly DbContext _context;

    public ResourceRepository(DbContext context)
    {
        _context = context;
    }

    public DbSet<Resource<TResource>> GetDbSet() => _context.Set<Resource<TResource>>();
    public IQueryable<Resource<TResource>> GetQuery() => _context.Set<Resource<TResource>>().AsQueryable();

    public virtual async Task<List<Resource<TResource>>> GetAllAsync(
        params Expression<Func<Resource<TResource>, bool>>[] filters)
    {
        var query = _context.Set<Resource<TResource>>().Where(r => !r.IsDeleted);
        if (filters is { Length: > 0 })
        {
            query = filters.Aggregate(query, (current, filter) => current.Where(filter));
        }
        return await query.ToListAsync();
    }

    public virtual async Task<Resource<TResource>> GetByIdAsync(string id)
    {
        var resource = await _context.Set<Resource<TResource>>().FindAsync(id);
        if (resource == null || resource.IsDeleted)
        {
            throw new NotImplementedException(id);
        }
        return resource;
    }

    public virtual async Task<Resource<TResource>> GetByOwnerAsync(string ownerId, params Expression<Func<Resource<TResource>, bool>>[] filters)
    {
        var query = _context.Set<Resource<TResource>>().AsQueryable();
        if (filters is { Length: > 0 })
        {
            query = filters.Aggregate(query, (current, filter) => current.Where(filter));
        }
        var resource = await query.FirstOrDefaultAsync(r => r.OwnerId == ownerId);
        if (resource == null || resource.IsDeleted)
        {
            throw new NotImplementedException();
        }
        return resource;
    }
    
    public virtual async Task<Resource<TResource>> FirstOrDefaultAsync(string ownerId, params Expression<Func<Resource<TResource>, bool>>[] filters)
    {
        var query = _context.Set<Resource<TResource>>().AsQueryable();
        if (filters is { Length: > 0 })
        {
            query = filters.Aggregate(query, (current, filter) => current.Where(filter));
        }
        var resource = await query.FirstOrDefaultAsync(r => r.OwnerId == ownerId);
        if (resource == null || resource.IsDeleted)
        {
            return null;
        }
        return resource;
    }
    
    public virtual async Task<List<Resource<TResource>>> GetListByOwnerAsync(string ownerId)
    {
        return await _context.Set<Resource<TResource>>().Where(r => r.OwnerId == ownerId && !r.IsDeleted)
            .OrderByDescending(r => r.UpdatedAt)
            .ToListAsync();
    }
    
    public async Task<int> GetCountByOwnerAsync(string ownerId, bool includeDeleted = false)
    {
        var resources = _context.Set<Resource<TResource>>()
            .Where(r => r.OwnerId == ownerId);
        
        if (!includeDeleted)
            resources = resources.Where(r => !r.IsDeleted);
        
        return await resources.CountAsync();
    }
    
    public virtual async Task<List<Resource<TResource>>> GetBySubjectId(string subjectId)
    {
        return await _context.Set<Resource<TResource>>().Where(r => r.SubjectId == subjectId && !r.IsDeleted)
            .ToListAsync();
    }
    
    public virtual async Task<Resource<TResource>> GetByOwnerIdAndSubjectId(string ownerId, string subjectId)
    {
        var resource = await _context.Set<Resource<TResource>>().FirstOrDefaultAsync(r => r.OwnerId == ownerId && r.SubjectId == subjectId);
        if (resource == null || resource.IsDeleted)
        {
            throw new NotImplementedException();
        }
        return resource;
    }
    
    public virtual async Task<Resource<TResource>> CreateResourceAsync(TResource data, string ownerId, string subjectId, string entityId)
    {
        var timestamp = DateTime.UtcNow;

        var resource = new Resource<TResource>
        {
            Id = Guid.NewGuid().ToString(),
            EntityId = entityId,
            OwnerId = ownerId,
            SubjectId = subjectId,
            CreatedAt = timestamp,
            UpdatedAt = timestamp,
            Version = 1,
            IsDeleted = false,
            Data = data
        };

        await _context.Set<Resource<TResource>>().AddAsync(resource);
        await _context.SaveChangesAsync();
        return resource;
    }
    
    public virtual async Task<Resource<TResource>> UpdateResourceAsync(string id, TResource data, bool isDeleted)
    {
        var storedResource = await _context.Set<Resource<TResource>>().FindAsync(id);
        if (storedResource == null || storedResource.IsDeleted)
            throw new NotImplementedException();

        storedResource.Version += 1;
        storedResource.UpdatedAt = DateTime.UtcNow;
        storedResource.Data = data;
        storedResource.IsDeleted = isDeleted;

        _context.Set<Resource<TResource>>().Update(storedResource);
        await _context.SaveChangesAsync();
        return storedResource;
    }
    
    public virtual async Task DeleteResourceAsync(string id)
    {
        var storedResource = await _context.Set<Resource<TResource>>().FindAsync(id);
        if (storedResource == null)
            return;

        _context.Set<Resource<TResource>>().Remove(storedResource);
        await _context.SaveChangesAsync();
    }
}
