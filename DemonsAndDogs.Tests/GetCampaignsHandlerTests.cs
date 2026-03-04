using AppConstants;
using Mediator.Mediator.Contracts;
using Mediator.Mediator.Handlers;
using Models.Resources;
using NSubstitute;

namespace DemonsAndDogs.Tests;

public class GetCampaignsHandlerTests
{
    private readonly IApiClient _apiClient;
    private readonly GetCampaignsHandler _handler;

    public GetCampaignsHandlerTests()
    {
        _apiClient = Substitute.For<IApiClient>();
        _handler = new GetCampaignsHandler(_apiClient);
    }

    [Fact]
    public async Task Handle_CallsCorrectApiEndpoint()
    {
        _apiClient.Get<IEnumerable<JsonResource>>(
            Arg.Any<string>(),
            Arg.Any<CancellationToken>())
            .Returns(new List<JsonResource>());

        await _handler.Handle(new GetCampaignsRequest("game-1"), CancellationToken.None);

        await _apiClient.Received(1).Get<IEnumerable<JsonResource>>(
            $"api/JsonResource/query?ResourceKind={ResourceKinds.Campaign}&GameId=game-1",
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ReturnsCampaigns()
    {
        var expected = new List<JsonResource>
        {
            new() { EntityId = "Campaign 1", ResourceKind = ResourceKinds.Campaign },
            new() { EntityId = "Campaign 2", ResourceKind = ResourceKinds.Campaign }
        };

        _apiClient.Get<IEnumerable<JsonResource>>(
            Arg.Any<string>(),
            Arg.Any<CancellationToken>())
            .Returns(expected);

        var result = await _handler.Handle(new GetCampaignsRequest("game-1"), CancellationToken.None);

        Assert.Equal(2, result.Count());
    }
}
