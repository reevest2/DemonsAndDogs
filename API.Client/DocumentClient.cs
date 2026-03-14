using API.Client.Abstraction;
using Models.Common;

namespace API.Client;

public class DocumentClient(IApiClient apiClient) : IDocumentClient
{
    public async Task<IEnumerable<DocumentResource>> GetByCampaignAsync(string campaignId, CancellationToken ct = default)
    {
        return await apiClient.Get<IEnumerable<DocumentResource>>($"api/document?campaignId={campaignId}", ct);
    }

    public async Task<DocumentResource?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        return await apiClient.Get<DocumentResource>($"api/document/{id}", ct);
    }

    public async Task<DocumentResource> CreateAsync(DocumentResource resource, CancellationToken ct = default)
    {
        return await apiClient.Post<DocumentResource, DocumentResource>("api/document", resource, ct);
    }

    public async Task<DocumentResource> UpdateAsync(DocumentResource resource, CancellationToken ct = default)
    {
        await apiClient.Put($"api/document/{resource.Id}", resource, ct);
        return resource;
    }

    public Task DeleteAsync(string id, CancellationToken ct = default) =>
        apiClient.Delete($"api/document/{id}", ct);
}
