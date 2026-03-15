using Microsoft.AspNetCore.Mvc;
using API.Extensions;
using Models.Common;
using API.Services.Documents;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentController(IDocumentService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DocumentResource>>> GetByCampaign([FromQuery] string campaignId, CancellationToken cancellationToken)
    {
        return Ok(await service.GetByCampaignAsync(campaignId, cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DocumentResource>> GetById(string id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result != null ? Ok(result) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<DocumentResource>> Create([FromBody] DocumentResource resource, CancellationToken cancellationToken)
    {
        var created = await service.CreateAsync(resource, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DocumentResource>> Update(string id, [FromBody] DocumentResource resource, CancellationToken cancellationToken)
    {
        if (id != resource.Id) return BadRequest("Id mismatch");
        var result = await service.UpdateAsync(resource, cancellationToken);
        return result.ToActionResult();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(id, cancellationToken);
        return result.ToActionResult();
    }
}
