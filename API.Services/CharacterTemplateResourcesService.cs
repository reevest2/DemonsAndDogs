using API.Services.Abstraction;
using DataAccess.Abstraction;
using Microsoft.Extensions.Logging;
using Models.Resources.Character;

namespace API.Services;

public class CharacterTemplateResourceService : IResourceService.ResourceService<CharacterTemplateData>, ICharacterTemplateResourceService
{
    public CharacterTemplateResourceService(IResourceRepository.IResourceRepository<CharacterTemplateData> resourceRepository, ILogger<ICharacterTemplateResourceService> logger) 
        : base(resourceRepository, logger)
    {
    }
}