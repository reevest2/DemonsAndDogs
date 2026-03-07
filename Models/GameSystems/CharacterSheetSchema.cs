namespace Models.GameSystems;

public record CharacterSheetSchema(
    string SystemId,
    List<SheetSection> Sections);

public record SheetSection(
    string Name,
    string DisplayName,
    List<SheetField> Fields);

public record SheetField(
    string Key,
    string Label,
    string Type, // e.g., "number", "text", "calculated"
    bool IsRequired,
    object? DefaultValue = null);
