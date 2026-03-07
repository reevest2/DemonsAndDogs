using Models.GameSystems;

namespace DemonsAndDogs.API.Tests.GameSystems.Builders;

public class AttackContextBuilder
{
    private string _weaponId = "longsword";
    private int _attackModifier = 0;
    private int? _targetArmorClass = 10;
    private Dictionary<string, int>? _additionalModifiers = null;

    public AttackContextBuilder WithWeaponId(string weaponId)
    {
        _weaponId = weaponId;
        return this;
    }

    public AttackContextBuilder WithAttackModifier(int modifier)
    {
        _attackModifier = modifier;
        return this;
    }

    public AttackContextBuilder WithTargetArmorClass(int? ac)
    {
        _targetArmorClass = ac;
        return this;
    }

    public AttackContextBuilder WithAdditionalModifiers(Dictionary<string, int>? modifiers)
    {
        _additionalModifiers = modifiers;
        return this;
    }

    public AttackContext Build()
    {
        return new AttackContext(
            _weaponId,
            _attackModifier,
            _targetArmorClass,
            _additionalModifiers
        );
    }
}
