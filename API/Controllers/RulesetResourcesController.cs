using API.Services.Abstraction;
using Models.Resources.Ruleset;

namespace API.Controllers;

public class RulesetResourcesController(IRulesetResourceService service)
    : ResourceControllerBase<RulesetData, IRulesetResourceService>(service);
    