using Models.Common;

namespace API.Client.Abstraction;

public interface IDocumentClient
{
    Task<IEnumerable<DocumentResource>> GetByCampaignAsync(string campaignId, CancellationToken ct = default);
    Task<DocumentResource?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<DocumentResource> CreateAsync(DocumentResource resource, CancellationToken ct = default);
    Task<DocumentResource> UpdateAsync(DocumentResource resource, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}
