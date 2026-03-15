using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using API.Services.Sessions.Contracts;
using Models;

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

        if (!result.IsSuccess)
        {
            var errorJson = JsonSerializer.Serialize(new { error = result.Error!.Code, message = result.Error.Message });
            await Response.WriteAsync($"data: {errorJson}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
            return;
        }

        var narration = result.Value!;

        if (narration.TokenStream != null)
        {
            await foreach (var token in narration.TokenStream.WithCancellation(cancellationToken))
            {
                await Response.WriteAsync($"data: {token}\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
        }
        else if (!string.IsNullOrEmpty(narration.Text))
        {
            await Response.WriteAsync($"data: {narration.Text}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
    }
}
