using System.Text.Json;
using AppConstants;
using MediatR;
using Mediator.Mediator.Contracts;
using Models.Resources;

namespace Mediator.Mediator.Handlers;

public class CreateSchemaHandler(IApiClient apiClient) : IRequestHandler<CreateSchemaRequest, JsonResource>
{
    public async Task<JsonResource> Handle(CreateSchemaRequest request, CancellationToken cancellationToken)
    {
        var jsonData = JsonSerializer.Deserialize<JsonElement>(request.JsonContent);

        var resource = new JsonResource
        {
            ResourceKind = ResourceKinds.Schema,
            Data = jsonData
        };

        var result = await apiClient.Post<JsonResource, JsonResource>(
            $"api/JsonResource/Create/User/{request.OwnerId}",
            resource,
            cancellationToken);

        return result;
    }
}
