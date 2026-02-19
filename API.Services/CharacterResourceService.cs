using API.Services.Abstraction;
using DataAccess.Abstraction;
using Microsoft.Extensions.Logging;
using Models.Character;

namespace API.Services;

public class CharacterResourceService : IResourceService.ResourceService<CharacterResource>, ICharacterResourceService
{
    public CharacterResourceService(IResourceRepository.IResourceRepository<CharacterResource> resourceRepository, ILogger<CharacterResourceService> logger) 
        : base(resourceRepository, logger)
    {
    }
}