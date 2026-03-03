using API.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Resources;

namespace API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class JsonResourceController(IJsonResourceService service) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<JsonResource>> GetById(string id)
    {
        var resource = await service.GetByIdAsync(id);
        if (resource == null)
            return NotFound();
        return Ok(resource);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JsonResource>>> GetAll()
    {
        var resources = await service.GetAllAsync();
        return Ok(resources);
    }

    [HttpGet("kind/{resourceKind}")]
    public async Task<ActionResult<IEnumerable<JsonResource>>> GetByResourceKind(string resourceKind)
    {
        var resources = await service.GetByResourceKindAsync(resourceKind);
        return Ok(resources);
    }

    [HttpGet("query")]
    public async Task<ActionResult<IEnumerable<JsonResource>>> Query(
        [FromQuery] string? entityId,
        [FromQuery] string? ownerId,
        [FromQuery] string? subjectId,
        [FromQuery] string? campaignId,
        [FromQuery] string? rulesetId,
        [FromQuery] string? gameId,
        [FromQuery] string? resourceKind)
    {
        var resources = await service.QueryAsync(q =>
        {
            if (!string.IsNullOrEmpty(entityId))
                q = q.Where(r => r.EntityId == entityId);
            if (!string.IsNullOrEmpty(ownerId))
                q = q.Where(r => r.OwnerId == ownerId);
            if (!string.IsNullOrEmpty(subjectId))
                q = q.Where(r => r.SubjectId == subjectId);
            if (!string.IsNullOrEmpty(campaignId))
                q = q.Where(r => r.CampaignId == campaignId);
            if (!string.IsNullOrEmpty(rulesetId))
                q = q.Where(r => r.RulesetId == rulesetId);
            if (!string.IsNullOrEmpty(gameId))
                q = q.Where(r => r.GameId == gameId);
            if (!string.IsNullOrEmpty(resourceKind))
                q = q.Where(r => r.ResourceKind == resourceKind);
            return q;
        });
        return Ok(resources);
    }

    [HttpPost]
    public async Task<ActionResult<JsonResource>> Create([FromBody] JsonResource resource)
    {
        var created = await service.CreateAsync(resource);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<JsonResource>> Update(string id, [FromBody] JsonResource resource)
    {
        resource.Id = id;
        var updated = await service.UpdateAsync(resource);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        await service.DeleteAsync(id);
        return NoContent();
    }
}
