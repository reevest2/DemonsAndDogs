using Models.GameSystems;

namespace Models.Interfaces;

public interface ICharacterSheetFactory
{
    CharacterSheetSchema Create(string systemId);
}
