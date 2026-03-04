using System.Text.Json;
using AppConstants;
using Mediator.Mediator.Contracts;
using Mediator.Mediator.Handlers;
using Models.Resources;
using NSubstitute;

namespace DemonsAndDogs.Tests;

public class CreateDocumentHandlerTests
{
    private readonly IApiClient _apiClient;
    private readonly CreateDocumentHandler _handler;

    public CreateDocumentHandlerTests()
    {
        _apiClient = Substitute.For<IApiClient>();
        _handler = new CreateDocumentHandler(_apiClient);
    }

    [Fact]
    public async Task Handle_SetsResourceKindToDocument()
    {
        var request = new CreateDocumentRequest("owner-1", "game-1", "campaign-1", "def-1", "My Sheet", """{"name":"Gandalf"}""");

        _apiClient.Post<JsonResource, JsonResource>(
            Arg.Any<string>(),
            Arg.Any<JsonResource>(),
            Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<JsonResource>());

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.Equal(ResourceKinds.Document, result.ResourceKind);
        Assert.Equal("owner-1", result.OwnerId);
        Assert.Equal("My Sheet", result.EntityId);
        Assert.Equal("game-1", result.GameId);
        Assert.Equal("campaign-1", result.CampaignId);
        Assert.Equal("def-1", result.Schema);
    }

    [Fact]
    public async Task Handle_CallsCorrectApiEndpoint()
    {
        var request = new CreateDocumentRequest("owner-1", "game-1", "campaign-1", "def-1", "My Sheet", """{"name":"Gandalf"}""");

        _apiClient.Post<JsonResource, JsonResource>(
            Arg.Any<string>(),
            Arg.Any<JsonResource>(),
            Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<JsonResource>());

        await _handler.Handle(request, CancellationToken.None);

        await _apiClient.Received(1).Post<JsonResource, JsonResource>(
            "api/JsonResource",
            Arg.Is<JsonResource>(r => r.GameId == "game-1" && r.EntityId == "My Sheet"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ThrowsOnInvalidJson()
    {
        var request = new CreateDocumentRequest("owner-1", "game-1", "campaign-1", "def-1", "My Sheet", "not-json");

        await Assert.ThrowsAsync<JsonException>(() =>
            _handler.Handle(request, CancellationToken.None));
    }
}
