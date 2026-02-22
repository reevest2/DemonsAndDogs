using API.Services.Abstraction;
using AppConstants;
using Microsoft.AspNetCore.Mvc;
using Models.Resources.Ruleset;

namespace API.Controllers;

[ApiController]
[Route($"api/{ResourceKeys.TemplateResources}")]
public class TemplateResourcesController(ITemplateResourceService service)
    : ResourceControllerBase<TemplateData, ITemplateResourceService>(service);