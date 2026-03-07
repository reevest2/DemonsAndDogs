using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mediator.Mediator.Contracts.Characters;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CharacterController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await mediator.Send(new GetCharactersRequest()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await mediator.Send(new GetCharacterRequest(id));
        return result != null ? Ok(result) : NotFound();
    }

    [HttpGet("system/{systemId}")]
    public async Task<IActionResult> GetBySystem(string systemId)
    {
        return Ok(await mediator.Send(new GetCharactersBySystemRequest(systemId)));
    }
}
