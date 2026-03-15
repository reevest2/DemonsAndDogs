using Models;
using Models.Common;

namespace API.Services.Documents;

public interface IDocumentService
{
    Task<IEnumerable<DocumentResource>> GetByCampaignAsync(string campaignId, CancellationToken cancellationToken = default);
    Task<DocumentResource?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<DocumentResource> CreateAsync(DocumentResource resource, CancellationToken cancellationToken = default);
    Task<Result<DocumentResource>> UpdateAsync(DocumentResource resource, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
