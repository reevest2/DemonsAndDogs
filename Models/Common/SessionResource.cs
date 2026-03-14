using AppConstants;

namespace Models.Common;

public record SessionResource : JsonResource
{
    public override string? ResourceKind { get; init; } = ResourceKinds.Session;
}
