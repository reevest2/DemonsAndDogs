using API.Client.Abstraction;
using Models.Common;

namespace API.Client;

public class CampaignClient(IApiClient apiClient) : ICampaignClient
{
    public async Task<IEnumerable<CampaignResource>> GetAllAsync(CancellationToken ct = default)
    {
        return await apiClient.Get<IEnumerable<CampaignResource>>("api/campaign", ct);
    }

    public async Task<CampaignResource?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        return await apiClient.Get<CampaignResource>($"api/campaign/{id}", ct);
    }
}
