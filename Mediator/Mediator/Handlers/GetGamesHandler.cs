using AppConstants;
using MediatR;
using Mediator.Mediator.Contracts;
using Models.Resources;

namespace Mediator.Mediator.Handlers;

public class GetGamesHandler(IApiClient apiClient) : IRequestHandler<GetGamesRequest, IEnumerable<JsonResource>>
{
    public async Task<IEnumerable<JsonResource>> Handle(GetGamesRequest request, CancellationToken cancellationToken)
    {
        return await apiClient.Get<IEnumerable<JsonResource>>(
            $"api/JsonResource/kind/{ResourceKinds.Game}",
            cancellationToken);
    }
}
