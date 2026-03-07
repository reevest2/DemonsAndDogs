using Models.GameSystems;

namespace API.Client.Abstraction;

public interface IGameSystemClient
{
    Task<IEnumerable<GameSystemDescriptor>> GetAllAsync(CancellationToken ct = default);
    Task<CharacterSheetSchema> GetSchemaAsync(string systemId, CancellationToken ct = default);
}
