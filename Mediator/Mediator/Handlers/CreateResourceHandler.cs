using System.Text.Json;
using AppConstants;
using MediatR;
using Mediator.Mediator.Contracts;
using Models.Common;

namespace Mediator.Mediator.Handlers;

public class CreateResourceHandler(IApiClient apiClient) : IRequestHandler<CreateResourceRequest, JsonResource>
{
    public async Task<JsonResource> Handle(CreateResourceRequest request, CancellationToken cancellationToken)
    {
        JsonResource resource = request.ResourceKind switch
        {
            ResourceKinds.Campaign => new CampaignResource(),
            ResourceKinds.Game => new GameResource(),
            ResourceKinds.Schema => new SchemaResource(),
            ResourceKinds.DocumentDefinition => new DocumentDefinitionResource(),
            ResourceKinds.Document => new DocumentResource(),
            ResourceKinds.Character => new CharacterResource(),
            _ => throw new ArgumentException($"Unsupported ResourceKind: {request.ResourceKind}")
        };

        resource = resource with
        {
            OwnerId = request.OwnerId,
            EntityId = request.EntityId,
            ResourceKind = request.ResourceKind,
            Schema = request.SchemaId,
            Data = JsonSerializer.Deserialize<JsonElement>(request.JsonContent)
        };

        return await apiClient.Post<JsonResource, JsonResource>(
            "api/JsonResource",
            resource,
            cancellationToken);
    }
}
