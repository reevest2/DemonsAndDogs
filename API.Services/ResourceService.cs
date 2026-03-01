using API.Services.Abstraction;
using DataAccess.Abstraction;
using Microsoft.Extensions.Logging;
using Models.Resources;
using Models.Resources.Abstract;

namespace API.Services;

public abstract class ResourceService<TResource>(
    IResourceRepository<TResource> resourceRepository,
    ILogger logger)
    : IResourceService<TResource> where TResource : ResourceBase
{
    protected readonly IResourceRepository<TResource> _resourceRepository = resourceRepository;
    protected readonly ILogger _logger = logger;

    public virtual async Task<List<TResource>> GetAll()
    {
        var items = await _resourceRepository.GetAllAsync();
        return items.ToList();
    }

    public virtual async Task<TResource> GetById(string ownerId, string resourceId)
    {
        var resource = await _resourceRepository.GetByIdAsync(resourceId);
        return resource;
    }

    public virtual async Task<TResource> GetFirstByOwner(string ownerId)
    {
        var resource = await _resourceRepository.FirstOrDefaultAsync(ownerId);
        return resource;
    }

    public virtual async Task<List<TResource>> GetAllByOwnerId(string ownerId)
    {
        var items = await _resourceRepository.GetListByOwnerAsync(ownerId);
        return items.ToList();
    }

    public virtual async Task<int> GetCountByOwnerId(string ownerId, bool includeDeleted = false)
    {
        return await _resourceRepository.GetCountByOwnerAsync(ownerId, includeDeleted);
    }

    public virtual async Task<TResource> GetByOwnerIdAndSubjectId(string ownerId, string subjectId)
    {
        var resource = await _resourceRepository.GetByOwnerIdAndSubjectId(ownerId, subjectId);
        return resource;
    }

    public virtual async Task<TResource> Create(string ownerId, TResource resource)
    {
        resource.OwnerId = ownerId;
        resource.Id ??= Guid.NewGuid().ToString();
        resource.CreatedAt = DateTime.UtcNow;
        resource.UpdatedAt = resource.CreatedAt;
        resource.Version = 1;
        resource.IsDeleted = false;

        var created = await _resourceRepository.CreateResourceAsync(resource);
        return created;
    }

    public virtual async Task<TResource> Update(string ownerId, string resourceId, TResource resource)
    {
        var storedResource = await _resourceRepository.GetByIdAsync(resourceId);
        var updated = await _resourceRepository.UpdateResourceAsync(storedResource);
        return updated;
    }

    public virtual async Task Delete(string ownerId, string resourceId, bool hardDelete = false)
    {
        var storedResource = await _resourceRepository.GetByIdAsync(resourceId);

        if (hardDelete)
        {
            await _resourceRepository.DeleteResourceAsync(storedResource);
            return;
        }

        await _resourceRepository.UpdateResourceAsync(storedResource);
    }
}