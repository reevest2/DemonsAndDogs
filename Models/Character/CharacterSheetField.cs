using System.Text.Json.Nodes;

namespace Models.Character;

public class CharacterSheetField
{
    public string Key { get; set; }
    public string Label { get; set; }
    public string Type { get; set; }
    public JsonNode Value { get; set; }
}