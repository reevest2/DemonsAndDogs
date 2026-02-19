using API.Services;
using API.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Models.Character;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CharacterResourcesController(ICharacterResourceService service) : ControllerBase
{
    [HttpGet("{resourceId}")]
    public async Task<ActionResult<CharacterResource>> GetById(string resourceId, CancellationToken ct)
    {
        var ownerId = User?.Identity?.Name ?? string.Empty;
        var resource = await service.GetById(ownerId, resourceId);
        if (resource is null) return NotFound();
        return Ok(resource);
    }

    [HttpGet]
    public async Task<ActionResult<List<CharacterResource>>> GetAll(CancellationToken ct)
    {
        var list = await service.GetAll();
        return Ok(list);
    }

    [HttpPost]
    public async Task<ActionResult<CharacterResource>> Create([FromBody] CharacterResource resource, CancellationToken ct)
    {
        var ownerId = User?.Identity?.Name ?? string.Empty;
        var created = await service.Create(ownerId, resource);
        return Ok(created);
    }

    [HttpPut("{resourceId}")]
    public async Task<ActionResult<CharacterResource>> Update(string resourceId, [FromBody] CharacterResource resource, CancellationToken ct)
    {
        var ownerId = User?.Identity?.Name ?? string.Empty;
        var updated = await service.Update(ownerId, resourceId, resource);
        return Ok(updated);
    }

    [HttpDelete("{resourceId}")]
    public async Task<IActionResult> Delete(string resourceId, CancellationToken ct)
    {
        var ownerId = User?.Identity?.Name ?? string.Empty;
        await service.Delete(ownerId, resourceId, hardDelete: false);
        return NoContent();
    }
}