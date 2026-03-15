using DataAccess.Abstraction;
using Models.Common;

namespace API.Services.Documents;

public class JsonDocumentService(IJsonResourceRepository repository) : IDocumentService
{
    public async Task<IEnumerable<DocumentResource>> GetByCampaignAsync(string campaignId, CancellationToken cancellationToken = default)
    {
        var results = await repository.QueryAsync(q =>
            q.OfType<DocumentResource>().Where(r => r.CampaignId == campaignId));
        return results.Cast<DocumentResource>();
    }

    public async Task<DocumentResource?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var resource = await repository.GetByIdAsync(id);
        return resource as DocumentResource;
    }

    public async Task<DocumentResource> CreateAsync(DocumentResource resource, CancellationToken cancellationToken = default)
    {
        var created = await repository.CreateAsync(resource);
        return (DocumentResource)created;
    }

    public async Task<DocumentResource> UpdateAsync(DocumentResource resource, CancellationToken cancellationToken = default)
    {
        var updated = await repository.UpdateAsync(resource);
        return (DocumentResource)updated;
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken = default) =>
        repository.DeleteAsync(id);
}
