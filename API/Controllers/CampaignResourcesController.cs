using API.Services.Abstraction;
using Models.Resources.Ruleset;

namespace API.Controllers;

public class CampaignResourcesController(ICampaignResourceService service)
    : ResourceControllerBase<CampaignData, ICampaignResourceService>(service);