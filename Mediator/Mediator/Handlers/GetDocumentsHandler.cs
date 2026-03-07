using AppConstants;
using MediatR;
using Mediator.Mediator.Contracts;
using Models.Common;

namespace Mediator.Mediator.Handlers;

public class GetDocumentsHandler(IApiClient apiClient) : IRequestHandler<GetDocumentsRequest, IEnumerable<JsonResource>>
{
    public async Task<IEnumerable<JsonResource>> Handle(GetDocumentsRequest request, CancellationToken cancellationToken)
    {
        var url = $"api/JsonResource/query?ResourceKind={ResourceKinds.Document}&GameId={request.GameId}";

        if (!string.IsNullOrEmpty(request.CampaignId))
            url += $"&CampaignId={request.CampaignId}";

        return await apiClient.Get<IEnumerable<JsonResource>>(url, cancellationToken);
    }
}
