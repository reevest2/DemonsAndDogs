using API.Services.Abstraction;
using DataAccess.Abstraction;
using IResourceService = System.ComponentModel.Design.IResourceService;
using API.Services.Abstraction;
using DataAccess.Abstraction;
using Microsoft.Extensions.Logging;
using Models.Resources.Ruleset;

namespace API.Services;

public class RulesetResourceService : ResourceService<RulesetData>, IRulesetResourceService
{
    public RulesetResourceService(IResourceRepository<RulesetData> resourceRepository, ILogger<RulesetResourceService> logger)
        : base(resourceRepository, logger)
    {
        
    }
}

public class CampaignResourceService : ResourceService<CampaignData>, ICampaignResourceService
{
    public CampaignResourceService(IResourceRepository<CampaignData> resourceRepository, ILogger<CampaignResourceService> logger)
        : base(resourceRepository, logger)
    {
    }
}

public class TemplateResourceService : ResourceService<TemplateData>, ITemplateResourceService
{
    public TemplateResourceService(IResourceRepository<TemplateData> resourceRepository, ILogger<TemplateResourceService> logger)
        : base(resourceRepository, logger)
    {
    }
}

public class EntityResourceService : ResourceService<EntityData>, IEntityResourceService
{
    public EntityResourceService(IResourceRepository<EntityData> resourceRepository, ILogger<EntityResourceService> logger)
        : base(resourceRepository, logger)
    {
    }
}