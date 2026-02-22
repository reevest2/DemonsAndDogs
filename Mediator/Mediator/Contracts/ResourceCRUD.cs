using Models.DTO;

namespace Mediator.Mediator;

public record ListResourcesQuery(
    string ResourceTypeKey,
    int Skip,
    int Take,
    string? SearchText,
    string? OrderBy,
    bool IncludeDeleted
) : MediatR.IRequest<PagedResult<ResourceDto>>;

public record GetResourceQuery(string ResourceTypeKey, string Id)
    : MediatR.IRequest<ResourceDto>;

public record CreateResourceCommand(
    string ResourceTypeKey,
    string? ResourceName,
    string? ResourceDescription,
    string? EntityId,
    string? SubjectId
) : MediatR.IRequest<string>;

public record UpdateResourceCommand(
    string ResourceTypeKey,
    string Id,
    string? ResourceName,
    string? ResourceDescription,
    bool IsDeleted
) : MediatR.IRequest;

public record DeleteResourceCommand(string ResourceTypeKey, string Id, bool HardDelete)
    : MediatR.IRequest;