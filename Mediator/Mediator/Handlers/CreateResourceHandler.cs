using System.Text.Json;
using AppConstants;
using MediatR;
using Mediator.Mediator.Contracts;
using Models.Resources;

namespace Mediator.Mediator.Handlers;

public class CreateResourceHandler(IApiClient apiClient) : IRequestHandler<CreateResourceRequest, JsonResource>
{
    public async Task<JsonResource> Handle(CreateResourceRequest request, CancellationToken cancellationToken)
    {
        var resource = new JsonResource
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
