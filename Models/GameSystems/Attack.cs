namespace Models.GameSystems;

public record AttackContext(
    string WeaponId,
    int AttackModifier,
    int? TargetArmorClass = null,
    Dictionary<string, int>? AdditionalModifiers = null);

public record AttackResult(
    int AttackRoll,
    int TotalAttackResult,
    bool IsHit,
    bool IsCriticalHit,
    int? DamageDealt = null,
    string? DamageType = null,
    string Message = "");
