using DataAccess.Abstraction;
using Models.Common;

namespace API.Services.Characters;

public class JsonCharacterService(IJsonResourceRepository repository) : ICharacterService
{
    public async Task<IEnumerable<CharacterResource>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var results = await repository.QueryAsync(q => q.OfType<CharacterResource>());
        return results.Cast<CharacterResource>();
    }

    public async Task<CharacterResource?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var results = await repository.QueryAsync(q => q.OfType<CharacterResource>().Where(r => r.Id == id));
        return results.Cast<CharacterResource>().FirstOrDefault();
    }

    public async Task<IEnumerable<CharacterResource>> GetBySystemIdAsync(string systemId, CancellationToken cancellationToken = default)
    {
        var results = await repository.QueryAsync(q => q.OfType<CharacterResource>().Where(r => r.GameId == systemId));
        return results.Cast<CharacterResource>();
    }
}
