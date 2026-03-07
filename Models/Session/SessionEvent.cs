using Models.GameSystems;

namespace Models.Session;

public record SessionEvent(
    string EventType,
    string Description,
    DateTime Timestamp,
    CheckResult? CheckResult = null,
    AttackResult? AttackResult = null);
