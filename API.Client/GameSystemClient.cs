using API.Client.Abstraction;
using Models.GameSystems;

namespace API.Client;

public class GameSystemClient(IApiClient apiClient) : IGameSystemClient
{
    public async Task<IEnumerable<GameSystemDescriptor>> GetAllAsync(CancellationToken ct = default)
    {
        return await apiClient.Get<IEnumerable<GameSystemDescriptor>>("api/GameSystem", ct);
    }

    public async Task<CharacterSheetSchema> GetSchemaAsync(string systemId, CancellationToken ct = default)
    {
        return await apiClient.Get<CharacterSheetSchema>($"api/GameSystem/{systemId}/schema", ct);
    }
}
