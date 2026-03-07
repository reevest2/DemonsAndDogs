# Game Session

## Overview
Game sessions represent an active instance of a TTRPG play session. A session is associated with a specific character name and a game system (e.g., `dnd5e`). 

The current implementation uses an in-memory `SessionStore` on the server for Phase 3, which persists for the lifetime of the API process.

## Session Flow

1. **Start Session**: 
   - The user enters a character name and selects a game system in the `SessionComponent`.
   - A `StartSessionRequest` is sent to the API.
   - The server creates a `SessionState`, generates a `SessionId`, and initializes the `CharacterSheetSchema` from the selected rulebook.
   - The `SessionState` is stored in the `SessionStore`.

2. **Perform Action**:
   - The user performs actions (e.g., "Roll Stealth" or "Attack") in the UI.
   - A `PerformActionRequest` is sent to the API with the `SessionId`, `ActionType`, and relevant context.
   - The server retrieves the session from the `SessionStore`.
   - The rulebook resolves the action and returns a `CheckResult` or `AttackResult`.
   - A `SessionEvent` is created, added to the session's event log, and the updated state is saved.
   - The `SessionEvent` is returned to the client to update the local UI state.

## Core Records

### SessionState (Models/Session/SessionState.cs)
Holds the complete state of an active game session.

```csharp
public record SessionState(
    string SessionId,
    string CharacterName,
    string SystemId,
    CharacterSheetSchema CharacterSheetSchema,
    IReadOnlyList<SessionEvent> EventLog);
```

### SessionEvent (Models/Session/SessionEvent.cs)
Represents a single action or event that occurred during the session.

```csharp
public record SessionEvent(
    string EventType,
    string Description,
    DateTime Timestamp,
    CheckResult? CheckResult = null,
    AttackResult? AttackResult = null);
```

### Contracts (Mediator/Contracts/Session/)
- `StartSessionRequest(string CharacterName, string SystemId)`
- `PerformActionRequest(string SessionId, ActionType ActionType, ...)`
- `ActionType` (Enum: `SkillCheck`, `Attack`)

## In-Memory Store Pattern
The `SessionStore` is a static class in the `Mediator` project that uses a `ConcurrentDictionary` to manage active sessions.

```csharp
// Mediator/Mediator/Handlers/Session/SessionStore.cs
public static class SessionStore
{
    public static ConcurrentDictionary<string, SessionState> Sessions { get; } = new();
}
```

This pattern allows MediatR handlers to maintain state across multiple requests without a database in the early development phases.

## Blazor Component Structure

The session UI is composed of several components in `DemonsAndDogs/Components/`:

- **SessionComponent**: The main container. It handles the initial "Start Session" form and then orchestrates the game session UI once a session is active. It manages the `SessionState` and calls the `ISessionClient`.
- **CharacterSheetComponent**: A read-only display of the character's stats based on the `CharacterSheetSchema` provided by the game system.
- **ActionLogComponent**: Displays a scrollable list of `SessionEvent` objects, showing the history of rolls and actions with appropriate styling (e.g., success/failure colors).

### Component Hierarchy
```
SessionComponent
├── CharacterSheetComponent (Left Sidebar)
├── ActionLogComponent (Center Column)
└── DM Narration Card (Right Sidebar - Placeholder for Phase 4)
```
