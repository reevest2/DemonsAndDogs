using API.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
[Route("User/{ownerId}/[controller]")]
public abstract class ResourceControllerBase<TResource, TService> : ControllerBase
    where TService : IResourceService.IResourceService<TResource>
{
    protected readonly TService Service;

    protected ResourceControllerBase(TService service)
    {
        Service = service;
    }

    [HttpGet]
    public virtual async Task<IActionResult> GetAll()
    {
        var response = await Service.GetAll();
        return Ok(response);
    }

    [HttpGet("{resourceId}")]
    public virtual async Task<IActionResult> GetById([FromRoute] string ownerId, [FromRoute] string resourceId)
    {
        var response = await Service.GetById(ownerId, resourceId);
        return Ok(response);
    }

    [HttpGet]
    public virtual async Task<IActionResult> GetAllByOwnerId([FromRoute] string ownerId)
    {
        var response = await Service.GetAllByOwnerId(ownerId);
        return Ok(response);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Create([FromRoute] string ownerId, [FromBody] TResource request)
    {
        var response = await Service.Create(ownerId, request);
        var controllerName = ControllerContext.ActionDescriptor.ControllerName;
        return Created($"User/{ownerId}/{controllerName}/{GetResourceId(response)}", response);
    }

    [HttpPut("{resourceId}")]
    public virtual async Task<IActionResult> Update(
        [FromRoute] string ownerId,
        [FromRoute] string resourceId,
        [FromBody] TResource resource)
    {
        var response = await Service.Update(ownerId, resourceId, resource);
        return Ok(response);
    }

    [HttpDelete("{resourceId}")]
    public virtual async Task<IActionResult> Delete([FromRoute] string ownerId, [FromRoute] string resourceId)
    {
        await Service.Delete(ownerId, resourceId);
        return NoContent();
    }

    [HttpGet("Count")]
    public virtual async Task<IActionResult> GetCountByOwnerId([FromRoute] string ownerId)
    {
        var response = await Service.GetCountByOwnerId(ownerId);
        return Ok(response);
    }

    protected virtual string GetResourceId(TResource resource)
    {
        var prop = typeof(TResource).GetProperty("Id");
        var value = prop?.GetValue(resource);
        return value?.ToString() ?? string.Empty;
    }
}