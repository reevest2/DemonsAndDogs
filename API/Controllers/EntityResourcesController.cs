using API.Services.Abstraction;
using Models.Resources.Ruleset;

namespace API.Controllers;

public class EntityResourcesController(IEntityResourceService service)
    : ResourceControllerBase<EntityData, IEntityResourceService>(service);