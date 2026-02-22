using API.Services.Abstraction;
using DataAccess.Abstraction;
using Microsoft.Extensions.Logging;
using Models.Character;
using Models.Resources;

namespace API.Services;

public class RulesetResourceService : IResourceService.ResourceService<RulesetResource>, IRulesetResourceService
{
    public RulesetResourceService(IResourceRepository.IResourceRepository<RulesetResource>resourceRepository, ILogger<RulesetResourceService> logger) 
        : base(resourceRepository, logger)
    {
    }
}
