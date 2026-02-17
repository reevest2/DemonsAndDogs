namespace Models.RuleSet;

public class SheetSectionDefinition
{
    public string Key { get; set; }
    public string Label { get; set; }
    public bool Required { get; set; }
    public int Order { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}