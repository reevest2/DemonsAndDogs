using API.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Resources;

namespace API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class JsonResourceController(IJsonResourceService service)
    : ResourceControllerBase<JsonResource, IJsonResourceService>(service)
{
}
