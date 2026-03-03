using AppConstants;
using Mediator.Mediator.Contracts;
using Mediator.Mediator.Handlers;
using Models.Resources;
using NSubstitute;

namespace DemonsAndDogs.Tests;

public class GetResourcesByKindHandlerTests
{
    private readonly IApiClient _apiClient;
    private readonly GetResourcesByKindHandler _handler;

    public GetResourcesByKindHandlerTests()
    {
        _apiClient = Substitute.For<IApiClient>();
        _handler = new GetResourcesByKindHandler(_apiClient);
    }

    [Fact]
    public async Task Handle_CallsCorrectApiEndpoint()
    {
        var request = new GetResourcesByKindRequest(ResourceKinds.Schema);

        _apiClient.Get<IEnumerable<JsonResource>>(
            Arg.Any<string>(),
            Arg.Any<CancellationToken>())
            .Returns([]);

        await _handler.Handle(request, CancellationToken.None);

        await _apiClient.Received(1).Get<IEnumerable<JsonResource>>(
            $"api/JsonResource/kind/{ResourceKinds.Schema}",
            Arg.Any<CancellationToken>());
    }
}
