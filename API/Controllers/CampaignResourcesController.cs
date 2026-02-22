using API.Services.Abstraction;
using AppConstants;
using Microsoft.AspNetCore.Mvc;
using Models.Resources.Ruleset;

namespace API.Controllers;

[ApiController]
[Route($"api/{ResourceKeys.CampaignResources}")]
public class CampaignResourcesController(ICampaignResourceService service)
    : ResourceControllerBase<CampaignData, ICampaignResourceService>(service);