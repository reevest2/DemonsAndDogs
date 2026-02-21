using API.Services.Abstraction;
using DataAccess.Abstraction;
using Microsoft.Extensions.Logging;
using Models.Character;
using Models.Resources;

public class CharacterTemplateResourceService : IResourceService.ResourceService<CharacterTemplateData>, ICharacterTemplateResourceService
{
    public CharacterTemplateResourceService(IResourceRepository.IResourceRepository<CharacterTemplateData> resourceRepository, ILogger<ICharacterTemplateResourceService> logger) 
        : base(resourceRepository, logger)
    {
    }
}