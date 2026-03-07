namespace Models.GameSystems;

public record SkillCheckContext(
    string CharacterId,
    string SkillId, 
    int AbilityModifier, 
    int ProficiencyBonus, 
    int? DifficultyClass = null, 
    Dictionary<string, int>? AdditionalModifiers = null);

public record CheckResult(
    int RollValue, 
    int TotalResult, 
    bool IsSuccess, 
    string Message);
