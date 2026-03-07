using Models.GameSystems;

namespace DemonsAndDogs.API.Tests.GameSystems.Builders;

public class SkillCheckContextBuilder
{
    private string _characterId = "test-character";
    private string _skillId = "stealth";
    private int _abilityModifier = 0;
    private int _proficiencyBonus = 0;
    private int? _difficultyClass = 10;
    private Dictionary<string, int>? _additionalModifiers = null;

    public SkillCheckContextBuilder WithCharacterId(string characterId)
    {
        _characterId = characterId;
        return this;
    }

    public SkillCheckContextBuilder WithSkillId(string skillId)
    {
        _skillId = skillId;
        return this;
    }

    public SkillCheckContextBuilder WithAbilityModifier(int modifier)
    {
        _abilityModifier = modifier;
        return this;
    }

    public SkillCheckContextBuilder WithProficiencyBonus(int bonus)
    {
        _proficiencyBonus = bonus;
        return this;
    }

    public SkillCheckContextBuilder WithDifficultyClass(int? dc)
    {
        _difficultyClass = dc;
        return this;
    }

    public SkillCheckContextBuilder WithAdditionalModifiers(Dictionary<string, int>? modifiers)
    {
        _additionalModifiers = modifiers;
        return this;
    }

    public SkillCheckContext Build()
    {
        return new SkillCheckContext(
            _characterId,
            _skillId,
            _abilityModifier,
            _proficiencyBonus,
            _difficultyClass,
            _additionalModifiers
        );
    }
}
