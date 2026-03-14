using MediatR;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Mediator.Mediator.Contracts.Campaigns;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CampaignResource>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetCampaignsRequest(), cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CampaignResource>> GetById(string id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCampaignRequest(id), cancellationToken);
        return result != null ? Ok(result) : NotFound();
    }
}
