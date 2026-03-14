using AppConstants;

namespace Models.Common;

public record GameResource : JsonResource
{
    public override string? ResourceKind { get; init; } = ResourceKinds.Game;
}
