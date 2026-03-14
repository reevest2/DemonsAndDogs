using AppConstants;

namespace Models.Common;

public record DocumentDefinitionResource : JsonResource
{
    public override string Kind { get; init; } = ResourceKinds.DocumentDefinition;
}
