using AppConstants;

namespace Models.Common;

public record SchemaResource : JsonResource
{
    public override string? ResourceKind { get; init; } = ResourceKinds.Schema;
}
