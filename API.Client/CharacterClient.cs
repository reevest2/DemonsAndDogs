using API.Client.Abstraction;
using Models.Common;

namespace API.Client;

public class CharacterClient(IApiClient apiClient) : ICharacterClient
{
    public async Task<IEnumerable<JsonResource>> GetAllAsync(CancellationToken ct = default)
    {
        return await apiClient.Get<IEnumerable<JsonResource>>("api/character", ct);
    }

    public async Task<JsonResource?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        return await apiClient.Get<JsonResource>($"api/character/{id}", ct);
    }

    public async Task<IEnumerable<JsonResource>> GetBySystemIdAsync(string systemId, CancellationToken ct = default)
    {
        return await apiClient.Get<IEnumerable<JsonResource>>($"api/character/system/{systemId}", ct);
    }
}
