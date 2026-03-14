using System.Text.Json;
using Models.GameSystems;

namespace Models.Interfaces;

public interface IRuleBook
{
    string SystemId { get; }
    string DisplayName { get; }
    CheckResult ResolveSkillCheck(SkillCheckContext context);
    AttackResult ResolveAttack(AttackContext context);
    CharacterSheetSchema GetCharacterSheetSchema();
    IReadOnlyDictionary<string, int> ExtractStats(JsonElement data);
}
