using System.Text.Json;
using AppConstants;
using MediatR;
using Mediator.Mediator.Contracts;
using Models.Common;

namespace Mediator.Mediator.Handlers;

public class CreateDocumentHandler(IApiClient apiClient) : IRequestHandler<CreateDocumentRequest, JsonResource>
{
    public async Task<JsonResource> Handle(CreateDocumentRequest request, CancellationToken cancellationToken)
    {
        var resource = new DocumentResource
        {
            OwnerId = request.OwnerId,
            EntityId = request.Name,
            GameId = request.GameId,
            CampaignId = request.CampaignId,
            Schema = request.DocumentDefinitionId,
            ResourceKind = ResourceKinds.Document,
            Data = JsonSerializer.Deserialize<JsonElement>(request.JsonContent)
        };

        return await apiClient.Post<JsonResource, JsonResource>(
            "api/JsonResource",
            resource,
            cancellationToken);
    }
}
