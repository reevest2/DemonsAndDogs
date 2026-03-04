using AppConstants;
using MediatR;
using Mediator.Mediator.Contracts;
using Models.Resources;

namespace Mediator.Mediator.Handlers;

public class GetCampaignsHandler(IApiClient apiClient) : IRequestHandler<GetCampaignsRequest, IEnumerable<JsonResource>>
{
    public async Task<IEnumerable<JsonResource>> Handle(GetCampaignsRequest request, CancellationToken cancellationToken)
    {
        return await apiClient.Get<IEnumerable<JsonResource>>(
            $"api/JsonResource/query?ResourceKind={ResourceKinds.Campaign}&GameId={request.GameId}",
            cancellationToken);
    }
}
