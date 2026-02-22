using DataAccess.Abstraction;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Models.Resources;
using Models.Resources.Abstract;

namespace API.Services.Abstraction;

public interface IResourceService
{
    public interface IResourceService<TResource>
    {
        Task<List<Resource<TResource>>> GetAll();
        Task<Resource<TResource>> GetById(string ownerId, string resourceId);
        Task<Resource<TResource>> GetFirstByOwner(string ownerId);
        Task<List<Resource<TResource>>> GetAllByOwnerId(string ownerId);
        Task<int> GetCountByOwnerId(string ownerId, bool includeDeleted = false);
        Task<Resource<TResource>> GetByOwnerIdAndSubjectId(string ownerId, string subjectId);
        Task<Resource<TResource>> Create(string ownerId, TResource resource);
        Task<Resource<TResource>> Update(string ownerId, string resourceId, TResource resource);
        Task Delete(string ownerId, string resourceId, bool hardDelete = false);
    }
    
   public abstract class ResourceService<TResource>(
       IResourceRepository.IResourceRepository<TResource> resourceRepository,
       ILogger logger)
       : IResourceService.IResourceService<TResource>
   {
    protected readonly IResourceRepository.IResourceRepository<TResource> _resourceRepository = resourceRepository;
    protected readonly ILogger _logger = logger;

    public virtual async Task<List<Resource<TResource>>> GetAll()
    {
        return await _resourceRepository.GetAllAsync();
    }
    public virtual async Task<Resource<TResource>> GetById(string ownerId, string resourceId)
    {
        var resource = await _resourceRepository.GetByIdAsync(resourceId);
        return resource;
    }

    public virtual async Task<Resource<TResource>> GetFirstByOwner(string ownerId)
    {
        var resource = await _resourceRepository.FirstOrDefaultAsync(ownerId);
        return resource;
    }
    
    public virtual async Task<List<Resource<TResource>>> GetAllByOwnerId(string ownerId)
    {
        return await _resourceRepository.GetListByOwnerAsync(ownerId);
    }
    
    public virtual async Task<int> GetCountByOwnerId(string ownerId, bool includeDeleted = false)
    {
        return await _resourceRepository.GetCountByOwnerAsync(ownerId, includeDeleted);
    }
    
    public virtual async Task<Resource<TResource>> GetByOwnerIdAndSubjectId(string ownerId, string subjectId)
    {
        return await _resourceRepository.GetByOwnerIdAndSubjectId(ownerId, subjectId);
    }
    
    public virtual async Task<Resource<TResource>> Create(string ownerId, TResource resource)
    {
        return await _resourceRepository.CreateResourceAsync(resource, ownerId, null, null);
    }
    
    public virtual async Task<Resource<TResource>> Update(string ownerId, string resourceId, TResource resource)
    {
        var storedResource = await _resourceRepository.GetByIdAsync(resourceId);
        return await _resourceRepository.UpdateResourceAsync(resourceId, resource);
    }
    
    public virtual async Task Delete(string ownerId, string resourceId, bool hardDelete = false)
    {
        var storedResource = await _resourceRepository.GetByIdAsync(resourceId);
        if (hardDelete)
        {
            await _resourceRepository.DeleteResourceAsync(resourceId);
        }
        else
        {
            await _resourceRepository.UpdateResourceAsync(resourceId, storedResource.Data, true);
        }
    }
}
}