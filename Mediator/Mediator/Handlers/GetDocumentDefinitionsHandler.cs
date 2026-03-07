using AppConstants;
using MediatR;
using Mediator.Mediator.Contracts;
using Models.Common;

namespace Mediator.Mediator.Handlers;

public class GetDocumentDefinitionsHandler(IApiClient apiClient) : IRequestHandler<GetDocumentDefinitionsRequest, IEnumerable<JsonResource>>
{
    public async Task<IEnumerable<JsonResource>> Handle(GetDocumentDefinitionsRequest request, CancellationToken cancellationToken)
    {
        return await apiClient.Get<IEnumerable<JsonResource>>(
            $"api/JsonResource/query?ResourceKind={ResourceKinds.DocumentDefinition}&GameId={request.GameId}",
            cancellationToken);
    }
}
