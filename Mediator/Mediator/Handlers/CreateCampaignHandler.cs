using System.Text.Json;
using AppConstants;
using MediatR;
using Mediator.Mediator.Contracts;
using Models.Resources;

namespace Mediator.Mediator.Handlers;

public class CreateCampaignHandler(IApiClient apiClient) : IRequestHandler<CreateCampaignRequest, JsonResource>
{
    public async Task<JsonResource> Handle(CreateCampaignRequest request, CancellationToken cancellationToken)
    {
        var data = new { name = request.Name, description = request.Description };

        var resource = new JsonResource
        {
            OwnerId = request.OwnerId,
            EntityId = request.Name,
            GameId = request.GameId,
            ResourceKind = ResourceKinds.Campaign,
            Data = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(data))
        };

        return await apiClient.Post<JsonResource, JsonResource>(
            "api/JsonResource",
            resource,
            cancellationToken);
    }
}
