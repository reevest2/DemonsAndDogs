using API.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestResourceController(ITestResourceService testResourceService) : ControllerBase
{
    public record CreateTestResourceRequest(string OwnerId, TestResource Data);
    [HttpPost]
    public async Task<ActionResult<TestResource>> Create(CreateTestResourceRequest request)
    {
        var result = await testResourceService.Create(request.OwnerId, request.Data);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<TestResource>> GetAll()
    {
        var result = await testResourceService.GetAll();
        return Ok(result);
    }
}