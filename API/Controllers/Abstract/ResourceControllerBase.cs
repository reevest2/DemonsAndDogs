using API.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Abstract;

[ApiController]
public abstract class ResourceControllerBase<TModel, TService>(TService service) : ControllerBase
    where TService : IResourceService.IResourceService<TModel>
{
    protected string OwnerId => "dev";

    [HttpGet("{resourceId}")]
    public async Task<ActionResult<TModel>> GetById(string resourceId)
    {
        var resource = await service.GetById(OwnerId, resourceId);
        if (resource is null) return NotFound();
        return Ok(resource);
    }

    [HttpGet]
    public async Task<ActionResult<List<TModel>>> GetAll()
    {
        var list = await service.GetAll();
        return Ok(list);
    }

    [HttpPost]
    public async Task<ActionResult<TModel>> Create([FromBody] TModel resource)
    {
        var created = await service.Create(OwnerId, resource);
        return Ok(created);
    }

    [HttpPut("{resourceId}")]
    public async Task<ActionResult<TModel>> Update(string resourceId, [FromBody] TModel resource)
    {
        var updated = await service.Update(OwnerId, resourceId, resource);
        return Ok(updated);
    }

    [HttpDelete("{resourceId}")]
    public async Task<IActionResult> Delete(string resourceId)
    {
        await service.Delete(OwnerId, resourceId, false);
        return NoContent();
    }
}