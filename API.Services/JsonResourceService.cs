using API.Services.Abstraction;
using DataAccess.Abstraction;
using Microsoft.Extensions.Logging;
using Models.Common;

namespace API.Services;

public class JsonResourceService(IJsonResourceRepository repository, ILogger<JsonResourceService> logger) : IJsonResourceService
{
    public async Task<JsonResource?> GetByIdAsync(string id)
    {
        logger.LogInformation("Getting JsonResource by Id: {Id}", id);
        return await repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<JsonResource>> GetAllAsync()
    {
        logger.LogInformation("Getting all JsonResources");
        return await repository.GetAllAsync();
    }

    public async Task<IEnumerable<JsonResource>> GetByResourceKindAsync(string resourceKind)
    {
        logger.LogInformation("Getting JsonResources by ResourceKind: {ResourceKind}", resourceKind);
        return await repository.GetByResourceKindAsync(resourceKind);
    }

    public async Task<IEnumerable<JsonResource>> QueryAsync(Func<IQueryable<JsonResource>, IQueryable<JsonResource>> query)
    {
        logger.LogInformation("Querying JsonResources");
        return await repository.QueryAsync(query);
    }

    public async Task<JsonResource> CreateAsync(JsonResource resource)
    {
        logger.LogInformation("Creating JsonResource with Id: {Id}", resource.Id);
        return await repository.CreateAsync(resource);
    }

    public async Task<JsonResource> UpdateAsync(JsonResource resource)
    {
        logger.LogInformation("Updating JsonResource with Id: {Id}", resource.Id);
        return await repository.UpdateAsync(resource);
    }

    public async Task DeleteAsync(string id)
    {
        logger.LogInformation("Deleting JsonResource with Id: {Id}", id);
        await repository.DeleteAsync(id);
    }
}
