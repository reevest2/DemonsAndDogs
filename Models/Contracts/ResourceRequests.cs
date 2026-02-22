using System.Text.Json;
namespace Models.Contracts;


public sealed record CreateResourceRequest(
    string ResourceTypeKey,
    string? ResourceName,
    string? ResourceDescription,
    string? EntityId,
    string? OwnerId,
    string? SubjectId,
    JsonElement? Data
);

public sealed record UpdateResourceRequest(
    string ResourceTypeKey,
    string Id,
    string? ResourceName,
    string? ResourceDescription,
    string? EntityId,
    string? OwnerId,
    string? SubjectId,
    bool IsDeleted,
    JsonElement? Data
);