using Models.Enums;

namespace Models.Catalog.Record;

public record ResourceTypeDefinition(
    ResourceType Type,
    string DisplayName,
    string Group,
    Type? EditorComponent
);