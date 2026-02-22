using Models.Resources.Ruleset;

namespace API.Services.Abstraction;

public interface IRulesetResourceService : IResourceService.IResourceService<RulesetData>
{
}

public interface ICampaignResourceService : IResourceService.IResourceService<CampaignData>
{
}

public interface ITemplateResourceService : IResourceService.IResourceService<TemplateData>
{
}

public interface IEntityResourceService : IResourceService.IResourceService<EntityData>
{
}
