using Models.Resources.Ruleset;

namespace API.Services.Abstraction;

public interface IRulesetResourceService : IResourceService<RulesetData>
{
}

public interface ICampaignResourceService : IResourceService<CampaignData>
{
}

public interface ITemplateResourceService : IResourceService<TemplateData>
{
}

public interface IEntityResourceService : IResourceService<EntityData>
{
}
