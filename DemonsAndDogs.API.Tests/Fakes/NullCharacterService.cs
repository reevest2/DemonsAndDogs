using API.Services.Abstraction;
using Models.Common;

namespace DemonsAndDogs.API.Tests.Fakes;

/// <summary>No-op ICharacterService for tests that don't exercise character loading.</summary>
public class NullCharacterService : ICharacterService
{
    public Task<IEnumerable<CharacterResource>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(Enumerable.Empty<CharacterResource>());

    public Task<CharacterResource?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        => Task.FromResult<CharacterResource?>(null);

    public Task<IEnumerable<CharacterResource>> GetBySystemIdAsync(string systemId, CancellationToken cancellationToken = default)
        => Task.FromResult(Enumerable.Empty<CharacterResource>());
}
