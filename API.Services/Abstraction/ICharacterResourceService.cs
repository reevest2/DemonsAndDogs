using Models.Character;
using Models.Resources;
using Models.Resources.Character;

namespace API.Services.Abstraction;

public interface ICharacterResourceService : IResourceService.IResourceService<CharacterData>
{
    
}