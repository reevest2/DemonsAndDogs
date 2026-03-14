using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mediator.Mediator.Contracts.Session;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NarrationController(IMediator mediator) : ControllerBase
{
    [HttpGet("stream/{sessionId}")]
    public async Task GetNarrationStream(string sessionId, CancellationToken cancellationToken)
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        var result = await mediator.Send(new NarrateActionRequest(sessionId), cancellationToken);

        if (result.TokenStream != null)
        {
            await foreach (var token in result.TokenStream.WithCancellation(cancellationToken))
            {
                await Response.WriteAsync($"data: {token}\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
        }
        else if (!string.IsNullOrEmpty(result.Text))
        {
            await Response.WriteAsync($"data: {result.Text}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
    }
}
