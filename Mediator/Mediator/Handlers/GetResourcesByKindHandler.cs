using AppConstants;
using MediatR;
using Mediator.Mediator.Contracts;
using Models.Common;

namespace Mediator.Mediator.Handlers;

public class GetResourcesByKindHandler(IApiClient apiClient) : IRequestHandler<GetResourcesByKindRequest, IEnumerable<JsonResource>>
{
    public async Task<IEnumerable<JsonResource>> Handle(GetResourcesByKindRequest request, CancellationToken cancellationToken)
    {
        return await apiClient.Get<IEnumerable<JsonResource>>(
            $"api/JsonResource/kind/{request.ResourceKind}",
            cancellationToken);
    }
}
