using DataAccess.Abstraction;
using Microsoft.Extensions.Logging;
using Models.Resources.Abstract;

namespace API.Services.Abstraction;

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

