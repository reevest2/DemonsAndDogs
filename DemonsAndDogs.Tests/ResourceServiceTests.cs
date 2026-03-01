using DataAccess.Abstraction;
using Microsoft.Extensions.Logging;
using Models.Resources;
using NSubstitute;

namespace DemonsAndDogs.Tests;

public class TestableResourceService : API.Services.ResourceService<JsonResource>
{
    public TestableResourceService(
        IResourceRepository<JsonResource> repo,
        ILogger logger) : base(repo, logger) { }
}

public class ResourceServiceTests
{
    private readonly IResourceRepository<JsonResource> _repository;
    private readonly TestableResourceService _service;

    public ResourceServiceTests()
    {
        _repository = Substitute.For<IResourceRepository<JsonResource>>();
        var logger = Substitute.For<ILogger>();
        _service = new TestableResourceService(_repository, logger);
    }

    [Fact]
    public async Task Create_SetsOwnerId()
    {
        var resource = new JsonResource();
        _repository.CreateResourceAsync(Arg.Any<JsonResource>()).Returns(ci => ci.Arg<JsonResource>());

        var result = await _service.Create("owner-1", resource);

        Assert.Equal("owner-1", result.OwnerId);
    }

    [Fact]
    public async Task Create_SetsDefaultMetadata()
    {
        var resource = new JsonResource();
        _repository.CreateResourceAsync(Arg.Any<JsonResource>()).Returns(ci => ci.Arg<JsonResource>());

        var before = DateTime.UtcNow;
        var result = await _service.Create("owner-1", resource);
        var after = DateTime.UtcNow;

        Assert.NotNull(result.Id);
        Assert.Equal(1, result.Version);
        Assert.False(result.IsDeleted);
        Assert.InRange(result.CreatedAt, before, after);
        Assert.Equal(result.CreatedAt, result.UpdatedAt);
    }

    [Fact]
    public async Task Create_PreservesExistingId()
    {
        var resource = new JsonResource { Id = "custom-id" };
        _repository.CreateResourceAsync(Arg.Any<JsonResource>()).Returns(ci => ci.Arg<JsonResource>());

        var result = await _service.Create("owner-1", resource);

        Assert.Equal("custom-id", result.Id);
    }

    [Fact]
    public async Task Create_CallsRepository()
    {
        var resource = new JsonResource();
        _repository.CreateResourceAsync(Arg.Any<JsonResource>()).Returns(ci => ci.Arg<JsonResource>());

        await _service.Create("owner-1", resource);

        await _repository.Received(1).CreateResourceAsync(Arg.Any<JsonResource>());
    }
}
