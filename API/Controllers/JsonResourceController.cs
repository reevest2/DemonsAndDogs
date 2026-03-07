using API.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Common;

namespace API.Controllers;

/// <summary>
/// Controller for managing JSON resources.
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class JsonResourceController(IJsonResourceService service) : ControllerBase
{
    /// <summary>
    /// Gets a resource by its ID.
    /// </summary>
    /// <param name="id">The resource ID.</param>
    /// <returns>The resource if found; otherwise, NotFound.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<JsonResource>> GetById(string id)
    {
        var resource = await service.GetByIdAsync(id);
        if (resource == null)
            return NotFound();
        return Ok(resource);
    }

    /// <summary>
    /// Gets all resources.
    /// </summary>
    /// <returns>A collection of all resources.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JsonResource>>> GetAll()
    {
        var resources = await service.GetAllAsync();
        return Ok(resources);
    }

    /// <summary>
    /// Gets resources by their kind.
    /// </summary>
    /// <param name="resourceKind">The kind of the resources.</param>
    /// <returns>A collection of resources of the specified kind.</returns>
    [HttpGet("kind/{resourceKind}")]
    public async Task<ActionResult<IEnumerable<JsonResource>>> GetByResourceKind(string resourceKind)
    {
        var resources = await service.GetByResourceKindAsync(resourceKind);
        return Ok(resources);
    }

    /// <summary>
    /// Queries resources based on multiple filters.
    /// </summary>
    /// <returns>A collection of resources matching the filters.</returns>
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
            if (!string.IsNullOrEmpty(entityId)) q = q.Where(r => r.EntityId == entityId);
            if (!string.IsNullOrEmpty(ownerId)) q = q.Where(r => r.OwnerId == ownerId);
            if (!string.IsNullOrEmpty(subjectId)) q = q.Where(r => r.SubjectId == subjectId);
            if (!string.IsNullOrEmpty(campaignId)) q = q.Where(r => r.CampaignId == campaignId);
            if (!string.IsNullOrEmpty(rulesetId)) q = q.Where(r => r.RulesetId == rulesetId);
            if (!string.IsNullOrEmpty(gameId)) q = q.Where(r => r.GameId == gameId);
            if (!string.IsNullOrEmpty(resourceKind)) q = q.Where(r => r.ResourceKind == resourceKind);
            return q;
        });
        return Ok(resources);
    }

    /// <summary>
    /// Creates a new resource.
    /// </summary>
    /// <param name="resource">The resource to create.</param>
    /// <returns>The created resource.</returns>
    [HttpPost]
    public async Task<ActionResult<JsonResource>> Create([FromBody] JsonResource resource)
    {
        var created = await service.CreateAsync(resource);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Updates an existing resource.
    /// </summary>
    /// <param name="id">The resource ID.</param>
    /// <param name="resource">The updated resource data.</param>
    /// <returns>The updated resource if found; otherwise, NotFound.</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<JsonResource>> Update(string id, [FromBody] JsonResource resource)
    {
        if (id != resource.Id)
            return BadRequest("ID mismatch.");
            
        var updated = await service.UpdateAsync(resource);
        if (updated == null)
            return NotFound();
        return Ok(updated);
    }

    /// <summary>
    /// Deletes a resource.
    /// </summary>
    /// <param name="id">The resource ID.</param>
    /// <returns>NoContent upon success.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        await service.DeleteAsync(id);
        return NoContent();
    }
}
