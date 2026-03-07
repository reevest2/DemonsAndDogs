using Models.Common;

namespace API.Client.Abstraction;

public interface ICharacterClient
{
    Task<IEnumerable<JsonResource>> GetAllAsync(CancellationToken ct = default);
    Task<JsonResource?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<IEnumerable<JsonResource>> GetBySystemIdAsync(string systemId, CancellationToken ct = default);
}
