using System.Text.Json;
using AppConstants;
using Mediator.Mediator.Contracts;
using Mediator.Mediator.Handlers;
using Models.Resources;
using NSubstitute;

namespace DemonsAndDogs.Tests;

public class CreateDocumentDefinitionHandlerTests
{
    private readonly IApiClient _apiClient;
    private readonly CreateDocumentDefinitionHandler _handler;

    public CreateDocumentDefinitionHandlerTests()
    {
        _apiClient = Substitute.For<IApiClient>();
        _handler = new CreateDocumentDefinitionHandler(_apiClient);
    }

    [Fact]
    public async Task Handle_SetsResourceKindToDocumentDefinition()
    {
        var request = new CreateDocumentDefinitionRequest("owner-1", "game-1", "Character Sheet", """{"sections":{}}""");

        _apiClient.Post<JsonResource, JsonResource>(
            Arg.Any<string>(),
            Arg.Any<JsonResource>(),
            Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<JsonResource>());

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.Equal(ResourceKinds.DocumentDefinition, result.ResourceKind);
        Assert.Equal("owner-1", result.OwnerId);
        Assert.Equal("Character Sheet", result.EntityId);
        Assert.Equal("game-1", result.GameId);
    }

    [Fact]
    public async Task Handle_CallsCorrectApiEndpoint()
    {
        var request = new CreateDocumentDefinitionRequest("owner-1", "game-1", "Character Sheet", """{"sections":{}}""");

        _apiClient.Post<JsonResource, JsonResource>(
            Arg.Any<string>(),
            Arg.Any<JsonResource>(),
            Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<JsonResource>());

        await _handler.Handle(request, CancellationToken.None);

        await _apiClient.Received(1).Post<JsonResource, JsonResource>(
            "api/JsonResource",
            Arg.Is<JsonResource>(r => r.GameId == "game-1" && r.EntityId == "Character Sheet"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ThrowsOnInvalidJson()
    {
        var request = new CreateDocumentDefinitionRequest("owner-1", "game-1", "Character Sheet", "not-json");

        await Assert.ThrowsAsync<JsonException>(() =>
            _handler.Handle(request, CancellationToken.None));
    }
}
