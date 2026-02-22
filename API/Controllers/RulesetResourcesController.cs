using API.Services.Abstraction;
using AppConstants;
using Microsoft.AspNetCore.Mvc;
using Models.Resources.Ruleset;

namespace API.Controllers;

[ApiController]
[Route($"api/{ResourceKeys.RulesetResources}")]
public class RulesetResourcesController(IRulesetResourceService service)
    : ResourceControllerBase<RulesetData, IRulesetResourceService>(service);
    