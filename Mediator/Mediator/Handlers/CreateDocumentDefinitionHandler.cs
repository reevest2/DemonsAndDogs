using System.Text.Json;
using AppConstants;
using MediatR;
using Mediator.Mediator.Contracts;
using Models.Resources;

namespace Mediator.Mediator.Handlers;

public class CreateDocumentDefinitionHandler(IApiClient apiClient) : IRequestHandler<CreateDocumentDefinitionRequest, JsonResource>
{
    public async Task<JsonResource> Handle(CreateDocumentDefinitionRequest request, CancellationToken cancellationToken)
    {
        var resource = new JsonResource
        {
            OwnerId = request.OwnerId,
            EntityId = request.Name,
            GameId = request.GameId,
            ResourceKind = ResourceKinds.DocumentDefinition,
            Data = JsonSerializer.Deserialize<JsonElement>(request.JsonContent)
        };

        return await apiClient.Post<JsonResource, JsonResource>(
            "api/JsonResource",
            resource,
            cancellationToken);
    }
}
