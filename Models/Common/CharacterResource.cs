using AppConstants;

namespace Models.Common;

public record CharacterResource : JsonResource
{
    public override string Kind { get; init; } = ResourceKinds.Character;
}
