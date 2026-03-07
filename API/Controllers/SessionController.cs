using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mediator.Mediator.Contracts.Session;
using Models.Session;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionController(IMediator mediator) : ControllerBase
{
    [HttpPost("start")]
    public async Task<IActionResult> Start([FromBody] StartSessionRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpPost("action")]
    public async Task<IActionResult> Action([FromBody] PerformActionRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }
}
