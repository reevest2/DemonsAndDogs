using API.Services.Abstraction;
using Models.Resources.Ruleset;

namespace API.Controllers;

public class TemplateResourcesController(ITemplateResourceService service)
    : ResourceControllerBase<TemplateData, ITemplateResourceService>(service);