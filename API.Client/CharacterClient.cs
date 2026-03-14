using API.Client.Abstraction;
using Models.Common;

namespace API.Client;

public class CharacterClient(IApiClient apiClient) : ICharacterClient
{
    public async Task<IEnumerable<CharacterResource>> GetAllAsync(CancellationToken ct = default)
    {
        return await apiClient.Get<IEnumerable<CharacterResource>>("api/character", ct);
    }

    public async Task<CharacterResource?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        return await apiClient.Get<CharacterResource>($"api/character/{id}", ct);
    }

    public async Task<IEnumerable<CharacterResource>> GetBySystemIdAsync(string systemId, CancellationToken ct = default)
    {
        return await apiClient.Get<IEnumerable<CharacterResource>>($"api/character/system/{systemId}", ct);
    }

    public async Task<IReadOnlyDictionary<string, int>> GetStatsAsync(string id, CancellationToken ct = default)
    {
        return await apiClient.Get<Dictionary<string, int>>($"api/character/{id}/stats", ct);
    }
}
