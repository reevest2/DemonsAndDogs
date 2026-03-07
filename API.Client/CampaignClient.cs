using API.Client.Abstraction;
using Models.Common;

namespace API.Client;

public class CampaignClient(IApiClient apiClient) : ICampaignClient
{
    public async Task<IEnumerable<JsonResource>> GetAllAsync(CancellationToken ct = default)
    {
        return await apiClient.Get<IEnumerable<JsonResource>>("api/campaign", ct);
    }

    public async Task<JsonResource?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        return await apiClient.Get<JsonResource>($"api/campaign/{id}", ct);
    }
}
