using AppConstants;
using Mediator.Mediator.Contracts;
using Mediator.Mediator.Handlers;
using Models.Common;
using NSubstitute;

namespace DemonsAndDogs.Tests;

public class GetDocumentsHandlerTests
{
    private readonly IApiClient _apiClient;
    private readonly GetDocumentsHandler _handler;

    public GetDocumentsHandlerTests()
    {
        _apiClient = Substitute.For<IApiClient>();
        _handler = new GetDocumentsHandler(_apiClient);
    }

    [Fact]
    public async Task Handle_CallsCorrectApiEndpointWithGameId()
    {
        _apiClient.Get<IEnumerable<JsonResource>>(
            Arg.Any<string>(),
            Arg.Any<CancellationToken>())
            .Returns(new List<JsonResource>());

        await _handler.Handle(new GetDocumentsRequest("game-1"), CancellationToken.None);

        await _apiClient.Received(1).Get<IEnumerable<JsonResource>>(
            $"api/JsonResource/query?ResourceKind={ResourceKinds.Document}&GameId=game-1",
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_IncludesCampaignIdWhenProvided()
    {
        _apiClient.Get<IEnumerable<JsonResource>>(
            Arg.Any<string>(),
            Arg.Any<CancellationToken>())
            .Returns(new List<JsonResource>());

        await _handler.Handle(new GetDocumentsRequest("game-1", "campaign-1"), CancellationToken.None);

        await _apiClient.Received(1).Get<IEnumerable<JsonResource>>(
            $"api/JsonResource/query?ResourceKind={ResourceKinds.Document}&GameId=game-1&CampaignId=campaign-1",
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ReturnsDocuments()
    {
        var expected = new List<JsonResource>
        {
            new DocumentResource { EntityId = "Doc 1", ResourceKind = ResourceKinds.Document },
            new DocumentResource { EntityId = "Doc 2", ResourceKind = ResourceKinds.Document }
        };

        _apiClient.Get<IEnumerable<JsonResource>>(
            Arg.Any<string>(),
            Arg.Any<CancellationToken>())
            .Returns(expected);

        var result = await _handler.Handle(new GetDocumentsRequest("game-1"), CancellationToken.None);

        Assert.Equal(2, result.Count());
    }
}
