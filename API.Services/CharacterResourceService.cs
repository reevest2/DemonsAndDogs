using API.Services.Abstraction;
using DataAccess.Abstraction;
using Microsoft.Extensions.Logging;
using Models.Resources;
using Models.Resources.Character;

namespace API.Services;

public class CharacterResourceService : IResourceService.ResourceService<CharacterData>, ICharacterResourceService
{
    public CharacterResourceService(IResourceRepository.IResourceRepository<CharacterData> resourceRepository, ILogger<CharacterResourceService> logger) 
        : base(resourceRepository, logger)
    {
    }
}