using Models.Enum;

namespace Models.Character;

public class CharacterBase
{
    public string Name { get; set; }
    public string Description { get; set; }
    public AbilityScore AbilityScores { get; set; }
    public CharacterClass Class { get; set; }
    public int Level { get; set; }
}