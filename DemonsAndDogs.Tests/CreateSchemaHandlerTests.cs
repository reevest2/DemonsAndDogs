using System.Text.Json;
using AppConstants;
using Mediator.Mediator.Contracts;
using Mediator.Mediator.Handlers;
using Models.Resources;
using NSubstitute;

namespace DemonsAndDogs.Tests;

public class CreateSchemaHandlerTests
{
    private readonly IApiClient _apiClient;
    private readonly CreateSchemaHandler _handler;

    public CreateSchemaHandlerTests()
    {
        _apiClient = Substitute.For<IApiClient>();
        _handler = new CreateSchemaHandler(_apiClient);
    }

    [Fact]
    public async Task Handle_SetsResourceKindToSchema()
    {
        var request = new CreateSchemaRequest("owner-1", "test", ResourceKinds.Character, """{"type":"object"}""");

        _apiClient.Post<JsonResource, JsonResource>(
            Arg.Any<string>(),
            Arg.Any<JsonResource>(),
            Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<JsonResource>());

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.Equal(ResourceKinds.Schema, result.ResourceKind);
        Assert.Equal("owner-1", result.OwnerId);
        Assert.Equal("test", result.EntityId);
    }

    [Fact]
    public async Task Handle_ParsesJsonContent()
    {
        var json = """{"type":"object","properties":{}}""";
        var request = new CreateSchemaRequest("owner-1", "test", ResourceKinds.Character, json);

        _apiClient.Post<JsonResource, JsonResource>(
            Arg.Any<string>(),
            Arg.Any<JsonResource>(),
            Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<JsonResource>());

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.Equal("object", result.Data.GetProperty("type").GetString());
        Assert.Equal("owner-1", result.OwnerId);
        Assert.Equal("test", result.EntityId);
    }

    [Fact]
    public async Task Handle_CallsCorrectApiEndpoint()
    {
        var request = new CreateSchemaRequest("owner-1", "test", ResourceKinds.Character, """{"type":"object"}""");

        _apiClient.Post<JsonResource, JsonResource>(
            Arg.Any<string>(),
            Arg.Any<JsonResource>(),
            Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<JsonResource>());

        await _handler.Handle(request, CancellationToken.None);

        await _apiClient.Received(1).Post<JsonResource, JsonResource>(
            "api/JsonResource",
            Arg.Is<JsonResource>(r => r.OwnerId == "owner-1" && r.EntityId == "test"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ThrowsOnInvalidJson()
    {
        var request = new CreateSchemaRequest("owner-1", "test", ResourceKinds.Character, "not-json");

        await Assert.ThrowsAsync<JsonException>(() =>
            _handler.Handle(request, CancellationToken.None));
    }
}
