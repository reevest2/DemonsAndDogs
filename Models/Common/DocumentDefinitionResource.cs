using AppConstants;

namespace Models.Common;

public record DocumentDefinitionResource : JsonResource
{
    public override string? ResourceKind { get; init; } = ResourceKinds.DocumentDefinition;
}
