using Models.Character;

namespace Models.Resources;

public class CharacterResource
{
    public ThumbnailMetadata Thumbnail { get; set; }
    public CharacterSheet CharacterSheet { get; set; }
}