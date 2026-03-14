using API.Client.Abstraction;
using Models.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DemonsAndDogs.Tests.Fakes;

public class FakeCharacterClient : ICharacterClient
{
    public IEnumerable<CharacterResource> Characters { get; set; } = new List<CharacterResource>();

    public Task<IEnumerable<CharacterResource>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult(Characters);

    public Task<CharacterResource?> GetByIdAsync(string id, CancellationToken ct = default)
        => throw new System.NotImplementedException();

    public Task<IEnumerable<CharacterResource>> GetBySystemIdAsync(string systemId, CancellationToken ct = default)
        => throw new System.NotImplementedException();

    public Task<IReadOnlyDictionary<string, int>> GetStatsAsync(string id, CancellationToken ct = default)
        => throw new System.NotImplementedException();
}
