using API.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestResourceController(ITestResourceService testResourceService) : ControllerBase
{
    public record CreateTestResourceRequest(TestResource Data, string? Key1 = null, string? Key2 = null, string? Key3 = null, string? OwnerId = null);

    [HttpPost]
    public async Task<ActionResult<TestResource>> Create(CreateTestResourceRequest request)
    {
        var result = await testResourceService.Create(request.Data, request.Key1, request.Key2, request.Key3, request.OwnerId);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<TestResource>> GetAll()
    {
        var result = await testResourceService.GetAll();
        return Ok(result);
    }
}
