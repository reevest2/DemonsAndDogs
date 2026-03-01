using API.Controllers;
using API.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Models.Contracts;
using Models.Resources;
using NSubstitute;

namespace DemonsAndDogs.API.Tests;

public class JsonResourceControllerTests
{
    private readonly IJsonResourceService _service;
    private readonly JsonResourceController _controller;

    public JsonResourceControllerTests()
    {
        _service = Substitute.For<IJsonResourceService>();
        _controller = new JsonResourceController(_service);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult()
    {
        _service.GetAll().Returns(new List<JsonResource>());

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsAssignableFrom<List<JsonResource>>(okResult.Value);
    }

    [Fact]
    public async Task GetAll_ReturnsAllResources()
    {
        var resources = new List<JsonResource>
        {
            new() { Id = "1" },
            new() { Id = "2" }
        };
        _service.GetAll().Returns(resources);

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsAssignableFrom<List<JsonResource>>(okResult.Value);
        Assert.Equal(2, returned.Count);
    }

    [Fact]
    public async Task GetAllByOwnerId_ReturnsOkWithResources()
    {
        var ownerId = "owner-1";
        var resources = new List<JsonResource>
        {
            new() { Id = "1", OwnerId = ownerId }
        };
        _service.GetAllByOwnerId(ownerId).Returns(resources);

        var result = await _controller.GetAllByOwnerId(new OwnerRouteParams(ownerId));

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsAssignableFrom<List<JsonResource>>(okResult.Value);
        Assert.Single(returned);
    }

    [Fact]
    public async Task GetCountByOwnerId_ReturnsOkWithCount()
    {
        var ownerId = "owner-1";
        _service.GetCountByOwnerId(ownerId).Returns(5);

        var result = await _controller.GetCountByOwnerId(new OwnerRouteParams(ownerId));

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(5, okResult.Value);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var ownerId = "owner-1";
        var id = "resource-1";

        var result = await _controller.Delete(new OwnerResourceRouteParams(ownerId, id));

        Assert.IsType<NoContentResult>(result);
        await _service.Received(1).Delete(ownerId, id);
    }
}
