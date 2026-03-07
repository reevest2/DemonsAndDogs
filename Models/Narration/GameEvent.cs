namespace Models.Narration;

public record GameEvent(
    string EventType, // e.g., "CombatStart", "SkillCheckSuccess", "NPCDeath"
    string Description,
    DateTime OccurredAt,
    string? SubjectId = null,
    Dictionary<string, string>? Metadata = null);

public record NarrationResult(
    string Text,
    string? AudioUrl = null,
    string? ImagePrompt = null,
    IAsyncEnumerable<string>? TokenStream = null);
