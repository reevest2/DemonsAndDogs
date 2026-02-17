namespace Models.Character;

public class CharacterSheetSection
{
    public string Key { get; set; }
    public string Title { get; set; }
    public List<CharacterSheetField> Fields { get; set; } = new();
}