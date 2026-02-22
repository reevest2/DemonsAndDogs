using API.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public virtual async Task<IActionResult> GetById([FromRoute] string ownerId, [FromRoute] string id)
    {
        var response = await Service.GetById(ownerId, id);
        return Ok(response);
    }

    [HttpGet(nameof(GetAllByOwnerId) + "/User/{ownerId}")]
    public virtual async Task<IActionResult> GetAllByOwnerId([FromRoute] string ownerId)
    {
        var response = await Service.GetAllByOwnerId(ownerId);
        return Ok(response);
    }

    [HttpPost(nameof(Create) + "/User/{ownerId}")]
    public virtual async Task<IActionResult> Create([FromRoute] string ownerId, [FromBody] TResource request)
    {
        var response = await Service.Create(ownerId, request);
        return Created($"{nameof(GetById)}/User/{ownerId}/{GetResourceId(response)}", response);
    }

    [HttpPut(nameof(Update) + "/User/{ownerId}/{id}")]
    public virtual async Task<IActionResult> Update([FromRoute] string ownerId, [FromRoute] string id, [FromBody] TResource resource)
    {
        var response = await Service.Update(ownerId, id, resource);
        return Ok(response);
    }

    [HttpDelete(nameof(Delete) + "/User/{ownerId}/{id}")]
    public virtual async Task<IActionResult> Delete([FromRoute] string ownerId, [FromRoute] string id)
    {
        await Service.Delete(ownerId, id);
        return NoContent();
    }

    [HttpGet(nameof(GetCountByOwnerId) + "/User/{ownerId}")]
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