using MediatR;
using Microsoft.AspNetCore.Mvc;
using API.Services.Sessions.Contracts;
using Models.Session;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionController(IMediator mediator) : ControllerBase
{
    [HttpPost("start")]
    public async Task<ActionResult<SessionState>> Start([FromBody] StartSessionRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("action")]
    public async Task<ActionResult<SessionEvent>> Action([FromBody] PerformActionRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{sessionId}")]
    public async Task<ActionResult<SessionState>> GetSession(string sessionId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetSessionRequest(sessionId), cancellationToken);
        return Ok(result);
    }
}
