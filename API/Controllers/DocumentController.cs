using MediatR;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Mediator.Mediator.Contracts.Documents;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DocumentResource>>> GetByCampaign([FromQuery] string campaignId, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetDocumentsByCampaignRequest(campaignId), cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DocumentResource>> GetById(string id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetDocumentRequest(id), cancellationToken);
        return result != null ? Ok(result) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<DocumentResource>> Create([FromBody] DocumentResource resource, CancellationToken cancellationToken)
    {
        var created = await mediator.Send(new CreateDocumentRequest(resource), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DocumentResource>> Update(string id, [FromBody] DocumentResource resource, CancellationToken cancellationToken)
    {
        if (id != resource.Id) return BadRequest("Id mismatch");
        var updated = await mediator.Send(new UpdateDocumentRequest(resource), cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteDocumentRequest(id), cancellationToken);
        return NoContent();
    }
}
