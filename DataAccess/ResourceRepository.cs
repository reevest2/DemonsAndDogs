using System.Linq.Expressions;
using DataAccess.Abstraction;
using Microsoft.EntityFrameworkCore;
using Models.Resources;

namespace DataAccess;

     public class ResourceRepository<TResource> : IResourceRepository.IResourceRepository<TResource> where TResource : ResourceBase
    {
        protected readonly DbContext _context;

        public ResourceRepository(DbContext context)
        {
            _context = context;
        }

        public DbSet<TResource> GetDbSet() => _context.Set<TResource>();
        
        public IQueryable<Resource<TResource>> GetQuery() => _context.Set<Resource<TResource>>().AsQueryable();

        public virtual async Task<List<TResource>> GetAllAsync(params Expression<Func<Resource<TResource>, bool>>[] filters)
        {
            var query = _context.Set<Resource<TResource>>().Where(r => !r.IsDeleted);
            if (filters is { Length: > 0 })
            {
                query = filters.Aggregate(query, (current, filter) => current.Where(filter));
            }
            return await query.Select(r => r.Data).ToListAsync();
        }

        public virtual async Task<TResource> GetByIdAsync(string id)
        {
            var resource = await _context.Set<Resource<TResource>>().FindAsync(id);
            if (resource == null || resource.IsDeleted)
            {
                throw new NotImplementedException(id);
            }
            return resource.Data;
        }

        public virtual async Task<TResource> GetByOwnerAsync(string ownerId, params Expression<Func<Resource<TResource>, bool>>[] filters)
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
            return resource.Data;
        }
        
        public virtual async Task<TResource> FirstOrDefaultAsync(string ownerId, params Expression<Func<Resource<TResource>, bool>>[] filters)
        {
            var query = _context.Set<Resource<TResource>>().AsQueryable();
            if (filters is { Length: > 0 })
            {
                query = filters.Aggregate(query, (current, filter) => current.Where(filter));
            }
            var resource = await query.FirstOrDefaultAsync(r => r.OwnerId == ownerId);
            if (resource == null || resource.IsDeleted)
            {
                return default;
            }
            return resource.Data;
        }
        
        public virtual async Task<List<TResource>> GetListByOwnerAsync(string ownerId)
        {
          return await _context.Set<Resource<TResource>>().Where(r => r.OwnerId == ownerId && !r.IsDeleted)
              .OrderByDescending(r => r.UpdatedAt)
              .Select(r => r.Data).ToListAsync();
        }
        
        public async Task<int> GetCountByOwnerAsync(string ownerId, bool includeDeleted = false)
        {
            var resources = _context.Set<Resource<TResource>>()
                .Where(r => r.OwnerId == ownerId);
            
            if (!includeDeleted)
                resources = resources.Where(r => !r.IsDeleted);
            
            return await resources.CountAsync();
        }

        public virtual async Task<List<TResource>> GetBySubjectId(string subjectId)
        {
            return await _context.Set<Resource<TResource>>().Where(r => r.SubjectId == subjectId && !r.IsDeleted)
                .Select(r => r.Data).ToListAsync();
        }

        public virtual async Task<TResource> GetByOwnerIdAndSubjectId(string ownerId, string subjectId)
        {
            var resource = await _context.Set<Resource<TResource>>().FirstOrDefaultAsync(r => r.OwnerId == ownerId && r.SubjectId == subjectId);
            if (resource == null || resource.IsDeleted)
            {
                throw new NotImplementedException();
            }
            return resource.Data;
        }

        public virtual async Task<TResource> CreateResourceAsync(TResource data)
        {
            var timestamp = DateTime.UtcNow;
            data.CreatedAt = timestamp;
            data.UpdatedAt = timestamp;
            if (string.IsNullOrWhiteSpace(data.Id) || data.Id == "string")
                data.Id = Guid.NewGuid().ToString();
            data.Version = 1;
            var resource = new Resource<TResource>
            {
                Data = data,
                OwnerId = data.OwnerId,
                SubjectId = data.SubjectId,
                UpdatedAt = data.UpdatedAt,
                CreatedAt = data.CreatedAt,
                Version = data.Version,
                Id = data.Id
            };

            await _context.Set<Resource<TResource>>().AddAsync(resource);
            await _context.SaveChangesAsync();
            resource = await _context.Set<Resource<TResource>>().FindAsync(data.Id);
            return resource.Data;
        }

        public virtual async Task<TResource> UpdateResourceAsync(TResource data)
        {
            try
            {
                var timestamp = DateTime.UtcNow;
                data.UpdatedAt = timestamp;
                var storedResource = await _context.Set<Resource<TResource>>().FindAsync(data.Id);
                if (data.Version != storedResource.Version)
                {
                    throw new NotImplementedException();
                }
                data.Version += 1;
                storedResource.Version = data.Version;
                storedResource.UpdatedAt = data.UpdatedAt;
                storedResource.Data = data;
                storedResource.IsDeleted = data.IsDeleted;
                _context.Set<Resource<TResource>>().Update(storedResource);
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException ex)
            {
                throw new NotImplementedException(ex.Message);
            }
            var resource = await _context.Set<Resource<TResource>>().FindAsync(data.Id);
            return resource.Data;
        }
        
        public virtual async Task DeleteResourceAsync(TResource data)
        {
            try
            {
                var storedResource = await _context.Set<Resource<TResource>>().FindAsync(data.Id);
                _context.Set<Resource<TResource>>().Remove(storedResource);
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }
    }
