using AppConstants;

namespace Models.Common;

public record SchemaResource : JsonResource
{
    public override string Kind { get; init; } = ResourceKinds.Schema;
}
