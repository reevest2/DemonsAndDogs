using AppConstants;
using Mediator.Mediator.Contracts;
using Mediator.Mediator.Handlers;
using Models.Resources;
using NSubstitute;

namespace DemonsAndDogs.Tests;

public class GetGamesHandlerTests
{
    private readonly IApiClient _apiClient;
    private readonly GetGamesHandler _handler;

    public GetGamesHandlerTests()
    {
        _apiClient = Substitute.For<IApiClient>();
        _handler = new GetGamesHandler(_apiClient);
    }

    [Fact]
    public async Task Handle_CallsCorrectApiEndpoint()
    {
        var expected = new List<JsonResource> { new() { EntityId = "D&D 5e", ResourceKind = ResourceKinds.Game } };

        _apiClient.Get<IEnumerable<JsonResource>>(
            Arg.Any<string>(),
            Arg.Any<CancellationToken>())
            .Returns(expected);

        await _handler.Handle(new GetGamesRequest(), CancellationToken.None);

        await _apiClient.Received(1).Get<IEnumerable<JsonResource>>(
            $"api/JsonResource/kind/{ResourceKinds.Game}",
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ReturnsGames()
    {
        var expected = new List<JsonResource>
        {
            new() { EntityId = "D&D 5e", ResourceKind = ResourceKinds.Game },
            new() { EntityId = "Warhammer", ResourceKind = ResourceKinds.Game }
        };

        _apiClient.Get<IEnumerable<JsonResource>>(
            Arg.Any<string>(),
            Arg.Any<CancellationToken>())
            .Returns(expected);

        var result = await _handler.Handle(new GetGamesRequest(), CancellationToken.None);

        Assert.Equal(2, result.Count());
    }
}
