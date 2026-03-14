using Models.Common;

namespace API.Client.Abstraction;

public interface ICharacterClient
{
    Task<IEnumerable<CharacterResource>> GetAllAsync(CancellationToken ct = default);
    Task<CharacterResource?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<IEnumerable<CharacterResource>> GetBySystemIdAsync(string systemId, CancellationToken ct = default);
    Task<IReadOnlyDictionary<string, int>> GetStatsAsync(string id, CancellationToken ct = default);
}
