using AppConstants;
using Mediator.Mediator.Contracts;
using Mediator.Mediator.Handlers;
using Models.Common;
using NSubstitute;

namespace DemonsAndDogs.Tests;

public class CreateCampaignHandlerTests
{
    private readonly IApiClient _apiClient;
    private readonly CreateCampaignHandler _handler;

    public CreateCampaignHandlerTests()
    {
        _apiClient = Substitute.For<IApiClient>();
        _handler = new CreateCampaignHandler(_apiClient);
    }

    [Fact]
    public async Task Handle_SetsResourceKindToCampaign()
    {
        var request = new CreateCampaignRequest("owner-1", "game-1", "Lost Mines", "A starter adventure");

        _apiClient.Post<JsonResource, JsonResource>(
            Arg.Any<string>(),
            Arg.Any<JsonResource>(),
            Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<JsonResource>());

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.Equal(ResourceKinds.Campaign, result.ResourceKind);
        Assert.Equal("owner-1", result.OwnerId);
        Assert.Equal("Lost Mines", result.EntityId);
        Assert.Equal("game-1", result.GameId);
    }

    [Fact]
    public async Task Handle_CallsCorrectApiEndpoint()
    {
        var request = new CreateCampaignRequest("owner-1", "game-1", "Lost Mines", "A starter adventure");

        _apiClient.Post<JsonResource, JsonResource>(
            Arg.Any<string>(),
            Arg.Any<JsonResource>(),
            Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<JsonResource>());

        await _handler.Handle(request, CancellationToken.None);

        await _apiClient.Received(1).Post<JsonResource, JsonResource>(
            "api/JsonResource",
            Arg.Is<JsonResource>(r => r.GameId == "game-1" && r.EntityId == "Lost Mines"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SerializesNameAndDescriptionInData()
    {
        var request = new CreateCampaignRequest("owner-1", "game-1", "Lost Mines", "A starter adventure");

        _apiClient.Post<JsonResource, JsonResource>(
            Arg.Any<string>(),
            Arg.Any<JsonResource>(),
            Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<JsonResource>());

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.Equal("Lost Mines", result.Data.GetProperty("name").GetString());
        Assert.Equal("A starter adventure", result.Data.GetProperty("description").GetString());
    }
}
