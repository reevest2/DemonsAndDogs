using AppConstants;
using Mediator.Mediator.Contracts;
using Mediator.Mediator.Handlers;
using Models.Common;
using NSubstitute;

namespace DemonsAndDogs.Tests;

public class GetDocumentDefinitionsHandlerTests
{
    private readonly IApiClient _apiClient;
    private readonly GetDocumentDefinitionsHandler _handler;

    public GetDocumentDefinitionsHandlerTests()
    {
        _apiClient = Substitute.For<IApiClient>();
        _handler = new GetDocumentDefinitionsHandler(_apiClient);
    }

    [Fact]
    public async Task Handle_CallsCorrectApiEndpoint()
    {
        _apiClient.Get<IEnumerable<JsonResource>>(
            Arg.Any<string>(),
            Arg.Any<CancellationToken>())
            .Returns(new List<JsonResource>());

        await _handler.Handle(new GetDocumentDefinitionsRequest("game-1"), CancellationToken.None);

        await _apiClient.Received(1).Get<IEnumerable<JsonResource>>(
            $"api/JsonResource/query?ResourceKind={ResourceKinds.DocumentDefinition}&GameId=game-1",
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ReturnsDefinitions()
    {
        var expected = new List<JsonResource>
        {
            new DocumentDefinitionResource { EntityId = "Character Sheet", ResourceKind = ResourceKinds.DocumentDefinition },
            new DocumentDefinitionResource { EntityId = "Spell List", ResourceKind = ResourceKinds.DocumentDefinition }
        };

        _apiClient.Get<IEnumerable<JsonResource>>(
            Arg.Any<string>(),
            Arg.Any<CancellationToken>())
            .Returns(expected);

        var result = await _handler.Handle(new GetDocumentDefinitionsRequest("game-1"), CancellationToken.None);

        Assert.Equal(2, result.Count());
    }
}
