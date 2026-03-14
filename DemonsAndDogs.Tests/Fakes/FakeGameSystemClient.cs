using API.Client.Abstraction;
using Models.GameSystems;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DemonsAndDogs.Tests.Fakes;

public class FakeGameSystemClient : IGameSystemClient
{
    public IEnumerable<GameSystemDescriptor> Systems { get; set; } = new List<GameSystemDescriptor>();

    public Task<IEnumerable<GameSystemDescriptor>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult(Systems);

    public Task<CharacterSheetSchema> GetSchemaAsync(string systemId, CancellationToken ct = default)
        => throw new System.NotImplementedException();
}
