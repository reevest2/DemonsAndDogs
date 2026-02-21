namespace Models.RuleSet.CharacterSheet;

public class CharacterSheetDefinition
{
    public string Key { get; set; }
    public string Name { get; set; }
    public List<SheetSectionDefinition> Sections { get; set; } = new();
}