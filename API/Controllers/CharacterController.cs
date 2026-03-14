using MediatR;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Mediator.Mediator.Contracts.Characters;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CharacterController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CharacterResource>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetCharactersRequest(), cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CharacterResource>> GetById(string id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCharacterRequest(id), cancellationToken);
        return result != null ? Ok(result) : NotFound();
    }

    [HttpGet("system/{systemId}")]
    public async Task<ActionResult<IEnumerable<CharacterResource>>> GetBySystem(string systemId, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetCharactersBySystemRequest(systemId), cancellationToken));
    }
}
