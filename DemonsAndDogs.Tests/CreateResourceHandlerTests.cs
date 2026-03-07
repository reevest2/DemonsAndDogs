using System.Text.Json;
using AppConstants;
using Mediator.Mediator.Contracts;
using Mediator.Mediator.Handlers;
using Models.Common;
using NSubstitute;

namespace DemonsAndDogs.Tests;

public class CreateResourceHandlerTests
{
    private readonly IApiClient _apiClient;
    private readonly CreateResourceHandler _handler;

    public CreateResourceHandlerTests()
    {
        _apiClient = Substitute.For<IApiClient>();
        _handler = new CreateResourceHandler(_apiClient);
    }

    [Fact]
    public async Task Handle_SetsCorrectResourceProperties()
    {
        var request = new CreateResourceRequest("owner-1", "test-resource", ResourceKinds.Character, "schema-1", """{"HP":10}""");

        _apiClient.Post<JsonResource, JsonResource>(
            Arg.Any<string>(),
            Arg.Any<JsonResource>(),
            Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<JsonResource>());

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.Equal(ResourceKinds.Character, result.ResourceKind);
        Assert.Equal("owner-1", result.OwnerId);
        Assert.Equal("test-resource", result.EntityId);
        Assert.Equal("schema-1", result.Schema);
        Assert.Equal(10, result.Data.GetProperty("HP").GetInt32());
    }

    [Fact]
    public async Task Handle_CallsCorrectApiEndpoint()
    {
        var request = new CreateResourceRequest("owner-1", "test-resource", ResourceKinds.Character, "schema-1", """{"HP":10}""");

        _apiClient.Post<JsonResource, JsonResource>(
            Arg.Any<string>(),
            Arg.Any<JsonResource>(),
            Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<JsonResource>());

        await _handler.Handle(request, CancellationToken.None);

        await _apiClient.Received(1).Post<JsonResource, JsonResource>(
            "api/JsonResource",
            Arg.Is<JsonResource>(r => 
                r.OwnerId == "owner-1" && 
                r.EntityId == "test-resource" && 
                r.ResourceKind == ResourceKinds.Character &&
                r.Schema == "schema-1"),
            Arg.Any<CancellationToken>());
    }
}
