using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mediator.Mediator.Contracts.GameSystems;
using Models.GameSystems;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameSystemController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameSystemDescriptor>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetGameSystemsRequest(), cancellationToken));
    }

    [HttpGet("{systemId}/schema")]
    public async Task<ActionResult<CharacterSheetSchema>> GetSchema(string systemId, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetCharacterSheetSchemaRequest(systemId), cancellationToken));
    }
}
