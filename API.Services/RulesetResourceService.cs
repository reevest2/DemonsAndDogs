using API.Services.Abstraction;
using DataAccess.Abstraction;
using Microsoft.Extensions.Logging;
using Models.Resources;
using Models.Resources.Ruleset;

namespace API.Services;

public class RulesetResourceService : IResourceService.ResourceService<RulesetResource>, IRulesetResourceService
{
    public RulesetResourceService(IResourceRepository.IResourceRepository<RulesetResource>resourceRepository, ILogger<RulesetResourceService> logger) 
        : base(resourceRepository, logger)
    {
    }
}
