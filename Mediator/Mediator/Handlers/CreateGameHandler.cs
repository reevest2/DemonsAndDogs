using System.Text.Json;
using AppConstants;
using MediatR;
using Mediator.Mediator.Contracts;
using Models.Resources;

namespace Mediator.Mediator.Handlers;

public class CreateGameHandler(IApiClient apiClient) : IRequestHandler<CreateGameRequest, JsonResource>
{
    public async Task<JsonResource> Handle(CreateGameRequest request, CancellationToken cancellationToken)
    {
        var data = new { name = request.Name, description = request.Description };

        var resource = new JsonResource
        {
            OwnerId = request.OwnerId,
            EntityId = request.Name,
            ResourceKind = ResourceKinds.Game,
            Data = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(data))
        };

        return await apiClient.Post<JsonResource, JsonResource>(
            "api/JsonResource",
            resource,
            cancellationToken);
    }
}
