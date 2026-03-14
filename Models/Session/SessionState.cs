using Models.GameSystems;

namespace Models.Session;

public record SessionState(
    string SessionId,
    string CharacterName,
    string SystemId,
    CharacterSheetSchema CharacterSheetSchema,
    IReadOnlyDictionary<string, int> Stats,
    IReadOnlyList<SessionEvent> EventLog);
