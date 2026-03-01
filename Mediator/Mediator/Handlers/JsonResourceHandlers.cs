using MediatR;
using Mediator.Mediator.Contracts;
using Models.Resources;

namespace Mediator.Mediator.Handlers;

public class GetJsonResourcesHandler(ApiClient apiClient) : IRequestHandler<GetJsonResourcesQuery, List<JsonResource>>
{
    public async Task<List<JsonResource>> Handle(GetJsonResourcesQuery request, CancellationToken cancellationToken)
    {
        return await apiClient.Get<List<JsonResource>>($"api/JsonResource/GetAllByOwnerId/User/{request.OwnerId}", cancellationToken);
    }
}

public class GetJsonResourceByIdHandler(ApiClient apiClient) : IRequestHandler<GetJsonResourceByIdQuery, JsonResource>
{
    public async Task<JsonResource> Handle(GetJsonResourceByIdQuery request, CancellationToken cancellationToken)
    {
        return await apiClient.Get<JsonResource>($"api/JsonResource/GetById/User/{request.OwnerId}/{request.ResourceId}", cancellationToken);
    }
}
