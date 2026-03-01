using API.Services;
using DataAccess.Abstraction;
using Microsoft.Extensions.Logging;
using Models.Resources;
using NSubstitute;

namespace DemonsAndDogs.API.Tests;

public class ResourceServiceTests
{
    private readonly IResourceRepository<JsonResource> _repository;
    private readonly JsonResourceService _service;

    public ResourceServiceTests()
    {
        _repository = Substitute.For<IResourceRepository<JsonResource>>();
        var logger = Substitute.For<ILogger<JsonResource>>();
        _service = new JsonResourceService(_repository, logger);
    }

    [Fact]
    public async Task GetAll_CallsRepository()
    {
        _repository.GetAllAsync().Returns(new List<JsonResource>());

        var result = await _service.GetAll();

        Assert.NotNull(result);
        await _repository.Received(1).GetAllAsync();
    }

    [Fact]
    public async Task GetAllByOwnerId_CallsRepositoryWithOwnerId()
    {
        var ownerId = "owner-1";
        _repository.GetListByOwnerAsync(ownerId).Returns(new List<JsonResource>());

        var result = await _service.GetAllByOwnerId(ownerId);

        Assert.NotNull(result);
        await _repository.Received(1).GetListByOwnerAsync(ownerId);
    }

    [Fact]
    public async Task GetCountByOwnerId_ReturnsCount()
    {
        var ownerId = "owner-1";
        _repository.GetCountByOwnerAsync(ownerId).Returns(3);

        var result = await _service.GetCountByOwnerId(ownerId);

        Assert.Equal(3, result);
    }

    [Fact]
    public async Task Create_SetsDefaultMetadata()
    {
        var ownerId = "owner-1";
        var resource = new JsonResource();

        _repository.CreateResourceAsync(Arg.Any<JsonResource>())
            .Returns(ci => ci.Arg<JsonResource>());

        var result = await _service.Create(ownerId, resource);

        Assert.Equal(ownerId, result.OwnerId);
        Assert.NotNull(result.Id);
        Assert.False(result.IsDeleted);
    }
}
