using System.Text.Json;
using MediatR;
using Models.DTO;

public record ListResourcesQuery(
    string ResourceTypeKey,
    int Skip,
    int Take,
    string? SearchText,
    string? OrderBy,
    bool IncludeDeleted,
    string? OwnerId
) : IRequest<PagedResult<ResourceDto>>;

public record GetResourceQuery(string ResourceTypeKey, string Id, string? OwnerId)
    : IRequest<ResourceDto>;

public record CreateResourceCommand(
    string ResourceTypeKey,
    string? ResourceName,
    string? ResourceDescription,
    string? EntityId,
    string? OwnerId,
    string? SubjectId,
    JsonElement? Data
) : IRequest<string>;

public record UpdateResourceCommand(
    string ResourceTypeKey,
    string Id,
    string? ResourceName,
    string? ResourceDescription,
    string? EntityId,
    string? OwnerId,
    string? SubjectId,
    bool IsDeleted,
    JsonElement? Data
) : IRequest;

public record DeleteResourceCommand(string ResourceTypeKey, string Id, string? OwnerId, bool HardDelete)
    : IRequest;