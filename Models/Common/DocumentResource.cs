using AppConstants;

namespace Models.Common;

public record DocumentResource : JsonResource
{
    public override string? ResourceKind { get; init; } = ResourceKinds.Document;
}
