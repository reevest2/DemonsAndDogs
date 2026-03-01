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
        var request = new CreateSchemaRequest("owner-1", "test", """{"type":"object"}""");

        _apiClient.Post<JsonResource, JsonResource>(
            Arg.Any<string>(),
            Arg.Any<JsonResource>(),
            Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<JsonResource>());

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.Equal(ResourceKinds.Schema, result.ResourceKind);
    }

    [Fact]
    public async Task Handle_ParsesJsonContent()
    {
        var json = """{"type":"object","properties":{}}""";
        var request = new CreateSchemaRequest("owner-1", "test", json);

        _apiClient.Post<JsonResource, JsonResource>(
            Arg.Any<string>(),
            Arg.Any<JsonResource>(),
            Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<JsonResource>());

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.Equal("object", result.Data.GetProperty("type").GetString());
    }

    [Fact]
    public async Task Handle_CallsCorrectApiEndpoint()
    {
        var request = new CreateSchemaRequest("owner-1", "test", """{"type":"object"}""");

        _apiClient.Post<JsonResource, JsonResource>(
            Arg.Any<string>(),
            Arg.Any<JsonResource>(),
            Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<JsonResource>());

        await _handler.Handle(request, CancellationToken.None);

        await _apiClient.Received(1).Post<JsonResource, JsonResource>(
            "api/JsonResource/Create/User/owner-1",
            Arg.Any<JsonResource>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ThrowsOnInvalidJson()
    {
        var request = new CreateSchemaRequest("owner-1", "test", "not-json");

        await Assert.ThrowsAsync<JsonException>(() =>
            _handler.Handle(request, CancellationToken.None));
    }
}
