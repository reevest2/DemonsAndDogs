using Models.Common;

namespace API.Services.Abstraction;

public interface ICharacterService
{
    Task<IEnumerable<CharacterResource>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CharacterResource?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CharacterResource>> GetBySystemIdAsync(string systemId, CancellationToken cancellationToken = default);
}
