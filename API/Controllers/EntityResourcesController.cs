using API.Services.Abstraction;
using AppConstants;
using Microsoft.AspNetCore.Mvc;
using Models.Resources.Ruleset;

namespace API.Controllers;

[ApiController]
[Route($"api/{ResourceKeys.EntityResources}")]
public class EntityResourcesController(IEntityResourceService service)
    : ResourceControllerBase<EntityData, IEntityResourceService>(service);