namespace Models.DTO;

public record ResourceListItemDto(
    string Id,
    string? EntityId,
    string? OwnerId,
    string? SubjectId,
    int Version,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool IsDeleted,
    string ResourceTypeKey,
    string? ResourceName,
    string? ResourceDescription
);

public record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount);