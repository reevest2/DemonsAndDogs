using API.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Contracts;

namespace API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public abstract class ResourceControllerBase<TResource, TService> : ControllerBase
    where TService : IResourceService<TResource>
{
    protected readonly TService Service;

    protected ResourceControllerBase(TService service)
    {
        Service = service;
    }

    [Authorize(Policy = "ValidateApiKey")]
    [HttpGet(nameof(GetAll))]
    public virtual async Task<IActionResult> GetAll()
    {
        var response = await Service.GetAll();
        return Ok(response);
    }

    [HttpGet(nameof(GetById) + "/User/{ownerId}/{id}")]
    public virtual async Task<IActionResult> GetById([FromRoute] OwnerResourceRouteParams routeParams)
    {
        var response = await Service.GetById(routeParams.OwnerId, routeParams.Id);
        return Ok(response);
    }

    [HttpGet(nameof(GetAllByOwnerId) + "/User/{ownerId}")]
    public virtual async Task<IActionResult> GetAllByOwnerId([FromRoute] OwnerRouteParams routeParams)
    {
        var response = await Service.GetAllByOwnerId(routeParams.OwnerId);
        return Ok(response);
    }

    [HttpPost(nameof(Create) + "/User/{ownerId}")]
    public virtual async Task<IActionResult> Create([FromRoute] OwnerRouteParams routeParams, [FromBody] TResource request)
    {
        var response = await Service.Create(routeParams.OwnerId, request);
        return Created($"{nameof(GetById)}/User/{routeParams.OwnerId}/{GetResourceId(response)}", response);
    }

    [HttpPut(nameof(Update) + "/User/{ownerId}/{id}")]
    public virtual async Task<IActionResult> Update([FromRoute] OwnerResourceRouteParams routeParams, [FromBody] TResource resource)
    {
        var response = await Service.Update(routeParams.OwnerId, routeParams.Id, resource);
        return Ok(response);
    }

    [HttpDelete(nameof(Delete) + "/User/{ownerId}/{id}")]
    public virtual async Task<IActionResult> Delete([FromRoute] OwnerResourceRouteParams routeParams)
    {
        await Service.Delete(routeParams.OwnerId, routeParams.Id);
        return NoContent();
    }

    [HttpGet(nameof(GetCountByOwnerId) + "/User/{ownerId}")]
    public virtual async Task<IActionResult> GetCountByOwnerId([FromRoute] OwnerRouteParams routeParams)
    {
        var response = await Service.GetCountByOwnerId(routeParams.OwnerId);
        return Ok(response);
    }

    protected virtual string GetResourceId(TResource resource)
    {
        var prop = typeof(TResource).GetProperty("Id");
        var value = prop?.GetValue(resource);
        return value?.ToString() ?? string.Empty;
    }
}
