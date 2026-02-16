using DataAccess.Abstraction;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Models.Resources;

namespace API.Services.Abstraction;

public interface IResourceService
{
     public interface IResourceService<TResource>
    {
        Task<List<TResource>> GetAll();
        Task<TResource> GetById(string ownerId, string resourceId);
        Task<TResource> GetFirstByOwner(string ownerId);
        Task<List<TResource>> GetAllByOwnerId(string ownerId);
        Task<int> GetCountByOwnerId(string ownerId, bool includeDeleted = false);
        Task<TResource> GetByOwnerIdAndSubjectId(string ownerId, string subjectId);
        Task<TResource> Create(string ownerId, TResource resource);
        Task<TResource> Update(string ownerId, string resourceId, TResource resource);
        Task Delete(string ownerId, string resourceId, bool hardDelete = false);
    }
    
    public abstract class ResourceService<TResource> : IResourceService<TResource> where TResource : ResourceBase
    {
        protected readonly IResourceRepository.IResourceRepository<TResource> _resourceRepository;
        protected readonly ILogger _logger;
        
        public ResourceService(IResourceRepository.IResourceRepository<TResource> resourceRepository, ILogger logger)
        {
            _resourceRepository = resourceRepository;
            _logger = logger;
        }

        public virtual async Task<List<TResource>> GetAll()
        {
            return await _resourceRepository.GetAllAsync();
        }
        public virtual async Task<TResource> GetById(string ownerId, string resourceId)
        {
            var resource = await _resourceRepository.GetByIdAsync(resourceId);
            return resource;
        }

        public virtual async Task<TResource> GetFirstByOwner(string ownerId)
        {
            return (await _resourceRepository.GetListByOwnerAsync(ownerId))?.FirstOrDefault();
        }
        
        public virtual async Task<List<TResource>> GetAllByOwnerId(string ownerId)
        {
            return await _resourceRepository.GetListByOwnerAsync(ownerId);
        }
        
        public virtual async Task<int> GetCountByOwnerId(string ownerId, bool includeDeleted = false)
        {
            return await _resourceRepository.GetCountByOwnerAsync(ownerId, includeDeleted);
        }
        
        public virtual async Task<TResource> GetByOwnerIdAndSubjectId(string ownerId, string subjectId)
        {
            return await _resourceRepository.GetByOwnerIdAndSubjectId(ownerId, subjectId);
        }

        public virtual async Task<TResource> Create(string ownerId, TResource resource)
        {
            resource.OwnerId = ownerId;
            return await _resourceRepository.CreateResourceAsync(resource);
        }

        public virtual async Task<TResource> Update(string ownerId, string resourceId, TResource resource)
        {
            var storedResource = await _resourceRepository.GetByIdAsync(resourceId);
            return await _resourceRepository.UpdateResourceAsync(resource);
        }

        public virtual async Task Delete(string ownerId, string resourceId, bool hardDelete = false)
        {
            var storedResource = await _resourceRepository.GetByIdAsync(resourceId);
            if (hardDelete)
            {
                await _resourceRepository.DeleteResourceAsync(storedResource);
            }
            else
            {
                storedResource.IsDeleted = true;
                await _resourceRepository.UpdateResourceAsync(storedResource);
            }
        }
    }
}