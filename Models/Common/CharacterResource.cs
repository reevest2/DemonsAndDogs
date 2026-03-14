using AppConstants;

namespace Models.Common;

public record CharacterResource : JsonResource
{
    public override string? ResourceKind { get; init; } = ResourceKinds.Character;
}
