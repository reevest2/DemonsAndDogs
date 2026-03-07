using System.Text.Json;
using AppConstants;
using MediatR;
using Mediator.Mediator.Contracts;
using Models.Common;

namespace Mediator.Mediator.Handlers;

public class CreateSchemaHandler(IApiClient apiClient) : IRequestHandler<CreateSchemaRequest, JsonResource>
{
    /// <summary>
    /// Handles the processing of a request to create a schema resource.
    /// </summary>
    /// <param name="request">The request containing details for creating the schema, including the JSON content and owner ID.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="JsonResource"/>.</returns>
    public async Task<JsonResource> Handle(CreateSchemaRequest request, CancellationToken cancellationToken)
    {
        var resource = new SchemaResource
        {
            OwnerId = request.OwnerId,
            EntityId = request.Name,
            SubjectId = request.ResourceKind,
            ResourceKind = ResourceKinds.Schema,
            Data = JsonSerializer.Deserialize<JsonElement>(request.JsonContent)
        };

        return await apiClient.Post<JsonResource, JsonResource>(
            "api/JsonResource",
            resource,
            cancellationToken);
    }
}
