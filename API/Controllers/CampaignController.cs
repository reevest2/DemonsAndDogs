using Microsoft.AspNetCore.Mvc;
using Models.Common;
using API.Services.Campaigns;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignController(ICampaignService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CampaignResource>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await service.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CampaignResource>> GetById(string id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result != null ? Ok(result) : NotFound();
    }
}
