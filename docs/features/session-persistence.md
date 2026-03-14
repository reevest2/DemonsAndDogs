# Session Persistence

## Overview
Persist active game sessions to the database so that sessions survive API restarts and can be resumed across devices.

## Goals
- Replace the in-memory `SessionStore` with a database-backed implementation using `IJsonResourceRepository`.
- Ensure `StartSession` saves the initial `SessionState` to the database.
- Ensure `PerformAction` saves the updated `SessionState` (with the new event appended) after each action.
- Ensure `GetSession` loads a session from the database if not found in memory.

## Constraints
- No magic strings — new resource kind constant belongs in `AppConstants/ResourceKinds`.
- All DTOs and new model types must be records, not classes.
- `ISessionStore` interface must not change — the new persistent implementation is a drop-in replacement.
- No DbContext access outside `DataAccess/` — all persistence goes through `IJsonResourceRepository`.
- All async methods accept `CancellationToken`.
- Services injected by interface only — never by concrete type.
- See [architecture.md](../core/architecture.md) and [data-model.md](../core/data-model.md) for patterns.

## Data Model
- New `SessionResource : JsonResource` record in `Models/Common/`. Fixed fields: `SessionId`, `CharacterName`, `SystemId`. The full `SessionState` (including `EventLog`) is serialized into `JsonResource.Data`.
- New discriminator value `ResourceKinds.Session = "Session"` in `AppConstants`.
- `DbContext` updated to map `SessionResource` via `.HasValue<SessionResource>(ResourceKinds.Session)`.
- No new EF Core migrations needed beyond adding the discriminator value (single-table inheritance already in place).

## Flow
1. `StartSessionHandler` creates a `SessionState`, sets it in `ISessionStore`, then calls `ISessionPersistence.SaveAsync(state, ct)`.
2. `PerformActionHandler` resolves the action, appends the event to the session, updates `ISessionStore`, then calls `ISessionPersistence.SaveAsync(state, ct)`.
3. `GetSessionHandler` calls `ISessionStore.TryGet`. On miss, calls `ISessionPersistence.LoadAsync(sessionId, ct)`, sets the result in `ISessionStore`, and returns it.
4. `ISessionPersistence` (new interface in `Models/Interfaces/`) abstracts save/load from `IJsonResourceRepository`.
5. `JsonSessionPersistence` (impl in `API.Services/`) uses `IJsonResourceRepository` to upsert/query `SessionResource` records.

## API Changes
- `ISessionPersistence` interface (new, in `Models/Interfaces/`):
  - `SaveAsync(SessionState state, CancellationToken ct) : Task`
  - `LoadAsync(string sessionId, CancellationToken ct) : Task<SessionState?>`
- `SessionResource` record (new, in `Models/Common/`):
  - `override string Kind => ResourceKinds.Session`
  - Fixed fields: `SessionId`, `CharacterName`, `SystemId`
- No new HTTP endpoints — existing `POST /session/start`, `POST /session/action`, `GET /session/{id}` are unchanged.

## UI Changes
None — this feature is entirely server-side.

## Test Cases

### Happy Path
- [ ] `SaveAsync_ValidSessionState_PersistsSessionResourceWithCorrectKind`
- [ ] `SaveAsync_ValidSessionState_DataContainsSerializedEventLog`
- [ ] `SaveAsync_ExistingSession_UpdatesExistingResourceRatherThanCreatingDuplicate`
- [ ] `LoadAsync_ExistingSessionId_ReturnsRehydratedSessionStateWithEventLog`
- [ ] `LoadAsync_ExistingSessionId_SessionStateMatchesOriginal`
- [ ] `StartSessionHandler_NewSession_SessionIsSavedToRepository`
- [ ] `PerformActionHandler_AfterAction_UpdatedSessionIsSavedToRepository`
- [ ] `GetSessionHandler_SessionNotInMemory_LoadsFromRepositoryAndCachesInStore`

### Edge Cases
- [ ] `SaveAsync_SessionWithEmptyEventLog_PersistsSuccessfully`
- [ ] `LoadAsync_SessionWithEmptyEventLog_ReturnsSessionStateWithEmptyList`
- [ ] `GetSessionHandler_SessionAlreadyInMemory_DoesNotCallRepository`

### Error Cases
- [ ] `LoadAsync_UnknownSessionId_ReturnsNull`
- [ ] `GetSessionHandler_SessionNotInMemoryOrRepository_ThrowsOrReturnsNotFound`

## Open Questions
- None — scope is read/write of session state only; no auth, no multi-user session sharing.

## Implementation Notes
<!-- Filled in during implementation -->
