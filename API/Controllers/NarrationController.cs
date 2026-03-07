using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mediator.Mediator.Contracts.Session;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NarrationController(IMediator mediator) : ControllerBase
{
    [HttpGet("stream/{sessionId}")]
    public async Task GetNarrationStream(string sessionId)
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        var result = await mediator.Send(new NarrateActionRequest(sessionId));

        if (result.TokenStream != null)
        {
            await foreach (var token in result.TokenStream.WithCancellation(HttpContext.RequestAborted))
            {
                await Response.WriteAsync($"data: {token}\n\n");
                await Response.Body.FlushAsync();
            }
        }
        else if (!string.IsNullOrEmpty(result.Text))
        {
            await Response.WriteAsync($"data: {result.Text}\n\n");
            await Response.Body.FlushAsync();
        }
    }
}