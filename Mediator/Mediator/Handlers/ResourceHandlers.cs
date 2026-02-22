using MediatR;
using Models.Contracts;
using Models.DTO;

public sealed class ListResourcesHandler : IRequestHandler<ListResourcesQuery, PagedResult<ResourceDto>>
{
    private readonly ApiClient _api;

    public ListResourcesHandler(ApiClient api)
    {
        _api = api;
    }

    public Task<PagedResult<ResourceDto>> Handle(ListResourcesQuery request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.OwnerId))
            throw new InvalidOperationException("OwnerId is required.");

        var qs =
            $"resourceTypeKey={Uri.EscapeDataString(request.ResourceTypeKey)}&" +
            $"skip={request.Skip}&take={request.Take}&" +
            $"searchText={Uri.EscapeDataString(request.SearchText ?? "")}&" +
            $"orderBy={Uri.EscapeDataString(request.OrderBy ?? "")}&" +
            $"includeDeleted={request.IncludeDeleted.ToString().ToLowerInvariant()}";

        return _api.Get<PagedResult<ResourceDto>>(
            $"api/Resources/ListResources/User/{Uri.EscapeDataString(request.OwnerId) }?{qs}",
            ct
        );
    }
}

public sealed class GetResourceHandler : IRequestHandler<GetResourceQuery, ResourceDto>
{
    private readonly ApiClient _api;

    public GetResourceHandler(ApiClient api)
    {
        _api = api;
    }

    public Task<ResourceDto> Handle(GetResourceQuery request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.OwnerId))
            throw new InvalidOperationException("OwnerId is required.");

        return _api.Get<ResourceDto>(
            $"api/Resources/GetResource/User/{Uri.EscapeDataString(request.OwnerId)}/{Uri.EscapeDataString(request.Id)}?resourceTypeKey={Uri.EscapeDataString(request.ResourceTypeKey)}",
            ct
        );
    }
}

public sealed class CreateResourceHandler : IRequestHandler<CreateResourceCommand, string>
{
    private readonly ApiClient _api;

    public CreateResourceHandler(ApiClient api)
    {
        _api = api;
    }

    public Task<string> Handle(CreateResourceCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.OwnerId))
            throw new InvalidOperationException("OwnerId is required.");

        var payload = new CreateResourceRequest(
            request.ResourceTypeKey,
            request.ResourceName,
            request.ResourceDescription,
            request.EntityId,
            request.OwnerId,
            request.SubjectId,
            request.Data
        );

        return _api.Post<CreateResourceRequest, string>(
            $"api/Resources/Create/User/{Uri.EscapeDataString(request.OwnerId)}",
            payload,
            ct
        );
    }
}

public sealed class UpdateResourceHandler : IRequestHandler<UpdateResourceCommand>
{
    private readonly ApiClient _api;

    public UpdateResourceHandler(ApiClient api)
    {
        _api = api;
    }

    public async Task Handle(UpdateResourceCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.OwnerId))
            throw new InvalidOperationException("OwnerId is required.");

        var payload = new UpdateResourceRequest(
            request.ResourceTypeKey,
            request.Id,
            request.ResourceName,
            request.ResourceDescription,
            request.EntityId,
            request.OwnerId,
            request.SubjectId,
            request.IsDeleted,
            request.Data
        );

        await _api.Put(
            $"api/Resources/Update/User/{Uri.EscapeDataString(request.OwnerId)}/{Uri.EscapeDataString(request.Id)}",
            payload,
            ct
        );
    }
}

public sealed class DeleteResourceHandler : IRequestHandler<DeleteResourceCommand>
{
    private readonly ApiClient _api;

    public DeleteResourceHandler(ApiClient api)
    {
        _api = api;
    }

    public async Task Handle(DeleteResourceCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.OwnerId))
            throw new InvalidOperationException("OwnerId is required.");

        await _api.Delete(
            $"api/Resources/Delete/User/{Uri.EscapeDataString(request.OwnerId)}/{Uri.EscapeDataString(request.Id)}?hardDelete={request.HardDelete.ToString().ToLowerInvariant()}&resourceTypeKey={Uri.EscapeDataString(request.ResourceTypeKey)}",
            ct
        );
    }
}