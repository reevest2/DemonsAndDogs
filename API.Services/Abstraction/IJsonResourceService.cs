using Models.Common;

namespace API.Services.Abstraction;

public interface IJsonResourceService
{
    Task<JsonResource?> GetByIdAsync(string id);
    Task<IEnumerable<JsonResource>> GetAllAsync();
    Task<IEnumerable<JsonResource>> GetByResourceKindAsync(string resourceKind);
    Task<IEnumerable<JsonResource>> QueryAsync(Func<IQueryable<JsonResource>, IQueryable<JsonResource>> query);
    Task<JsonResource> CreateAsync(JsonResource resource);
    Task<JsonResource> UpdateAsync(JsonResource resource);
    Task DeleteAsync(string id);
}
