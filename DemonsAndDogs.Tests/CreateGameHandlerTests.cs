using AppConstants;
using Mediator.Mediator.Contracts;
using Mediator.Mediator.Handlers;
using Models.Common;
using NSubstitute;

namespace DemonsAndDogs.Tests;

public class CreateGameHandlerTests
{
    private readonly IApiClient _apiClient;
    private readonly CreateGameHandler _handler;

    public CreateGameHandlerTests()
    {
        _apiClient = Substitute.For<IApiClient>();
        _handler = new CreateGameHandler(_apiClient);
    }

    [Fact]
    public async Task Handle_SetsResourceKindToGame()
    {
        var request = new CreateGameRequest("owner-1", "D&D 5e", "Fantasy RPG");

        _apiClient.Post<JsonResource, JsonResource>(
            Arg.Any<string>(),
            Arg.Any<JsonResource>(),
            Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<JsonResource>());

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.Equal(ResourceKinds.Game, result.ResourceKind);
        Assert.Equal("owner-1", result.OwnerId);
        Assert.Equal("D&D 5e", result.EntityId);
    }

    [Fact]
    public async Task Handle_CallsCorrectApiEndpoint()
    {
        var request = new CreateGameRequest("owner-1", "D&D 5e", "Fantasy RPG");

        _apiClient.Post<JsonResource, JsonResource>(
            Arg.Any<string>(),
            Arg.Any<JsonResource>(),
            Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<JsonResource>());

        await _handler.Handle(request, CancellationToken.None);

        await _apiClient.Received(1).Post<JsonResource, JsonResource>(
            "api/JsonResource",
            Arg.Is<JsonResource>(r => r.OwnerId == "owner-1" && r.EntityId == "D&D 5e"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SerializesNameAndDescriptionInData()
    {
        var request = new CreateGameRequest("owner-1", "D&D 5e", "Fantasy RPG");

        _apiClient.Post<JsonResource, JsonResource>(
            Arg.Any<string>(),
            Arg.Any<JsonResource>(),
            Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<JsonResource>());

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.Equal("D&D 5e", result.Data.GetProperty("name").GetString());
        Assert.Equal("Fantasy RPG", result.Data.GetProperty("description").GetString());
    }
}
