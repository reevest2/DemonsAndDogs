using Models.Common;

namespace API.Services.Abstraction;

public interface ICharacterService
{
    Task<IEnumerable<CharacterResource>> GetAllAsync();
    Task<CharacterResource?> GetByIdAsync(string id);
    Task<IEnumerable<CharacterResource>> GetBySystemIdAsync(string systemId);
}