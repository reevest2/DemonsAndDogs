# Real Character Stats

## Overview
Replace the hardcoded default stats (all 10s) displayed in character sheets with real stat values read from `CharacterResource.Data`. Affects two surfaces: the active game session character sheet, and the character detail page.

## Goals
- When a session starts, load the selected character's real stats from `CharacterResource.Data` and store them in `SessionState.Stats`.
- When viewing a character's detail page, fetch and display real stats from `CharacterResource.Data`.
- Keep extraction logic inside `IRuleBook` — no game-system-specific parsing in controllers, handlers, or UI.

## Constraints
- No magic strings — stat field keys are defined by the schema (`SheetField.Key`) in the relevant `IRuleBook`.
- All DTOs and domain models must be records; `SessionState` changes accordingly.
- Services injected by interface only.
- Business logic (stat extraction) lives in `API.Services` via `IRuleBook`, not in controllers or Blazor components.
- All async methods accept `CancellationToken`.
- Controller actions remain one-liners: `return Ok(await mediator.Send(request, ct))`.

## Data Model

### Updated Seed Data
The `DbSeeder` character records currently use abbreviated JSON keys (`str`, `dex`). Update them to use full schema field keys so `ExtractStats` is a direct lookup with no mapping:

```json
{
  "name": "Gimli",
  "race": "Dwarf",
  "class": "Fighter",
  "strength": 18, "dexterity": 12, "constitution": 16,
  "intelligence": 10, "wisdom": 12, "charisma": 8,
  "hp": 45, "ac": 16
}
```

### `IRuleBook` — new method
```csharp
IReadOnlyDictionary<string, int> ExtractStats(JsonElement data);
```
Iterates over the schema returned by `GetCharacterSheetSchema()`, looks up each `SheetField.Key` in `data`, and returns a dict of field key → integer value. Falls back to the field's `DefaultValue` (cast to `int`) if the key is absent or non-numeric.

### `SessionState` — new property
```csharp
public record SessionState(
    string SessionId,
    string CharacterName,
    string SystemId,
    CharacterSheetSchema CharacterSheetSchema,
    IReadOnlyDictionary<string, int> Stats,   // NEW
    IReadOnlyList<SessionEvent> EventLog);
```

### `StartSessionRequest` — new field
```csharp
public record StartSessionRequest(string CharacterId, string CharacterName, string SystemId)
    : IRequest<SessionState>;
```
`CharacterId` is the `CharacterResource.Id` — used by `StartSessionHandler` to load the real character resource.

### No new `JsonResource` subtypes — `CharacterResource.Data` already carries stats.

## Flow

### Session Start
1. `NewSessionComponent` already holds `character.Id` — pass it as `CharacterId` in `StartSessionRequest`.
2. `StartSessionHandler`:
   a. Look up rulebook via `IGameSystemRegistry`.
   b. Fetch `CharacterResource` via `ICharacterService.GetByIdAsync(request.CharacterId, ct)`.
   c. Call `ruleBook.ExtractStats(character.Data)` → `IReadOnlyDictionary<string, int> stats`.
   d. Build `SessionState` with the real `stats` dict.
   e. Store and persist as before.
3. `SessionComponent` passes `_sessionState.Stats` to `CharacterSheetComponent`.
4. `CharacterSheetComponent` renders `Stats[field.Key]` when `Stats` is set, otherwise `field.DefaultValue`.

### Character Detail Page
1. `CharacterDetailComponent` calls `CharacterClient.GetStatsAsync(characterId, ct)` (new client method).
2. Client calls `GET /api/character/{id}/stats`.
3. `GetCharacterStatsHandler`:
   a. Fetches `CharacterResource` via `ICharacterService.GetByIdAsync(id, ct)`.
   b. Determines system from `character.GameId` → looks up rulebook.
   c. Calls `ruleBook.ExtractStats(character.Data)`.
   d. Returns the stats dict.
4. `CharacterDetailComponent` passes the stats dict to `CharacterSheetComponent`.

## API Changes

### New MediatR contract
```csharp
// Mediator/Contracts/Characters/CharacterRequests.cs  (append)
public record GetCharacterStatsRequest(string CharacterId)
    : IRequest<IReadOnlyDictionary<string, int>>;
```

### New MediatR handler
```
Mediator/Handlers/Characters/CharacterHandlers.cs  (append)
```
`GetCharacterStatsHandler(ICharacterService, IGameSystemRegistry)` — fetches character, looks up rulebook, returns `ExtractStats(character.Data)`. Returns an empty dict if character not found.

### New controller action
```csharp
// CharacterController
[HttpGet("{id}/stats")]
public async Task<ActionResult<IReadOnlyDictionary<string, int>>> GetStats(string id, CancellationToken ct)
    => Ok(await mediator.Send(new GetCharacterStatsRequest(id), ct));
```

### Updated `ICharacterClient` (API.Client)
```csharp
Task<IReadOnlyDictionary<string, int>> GetStatsAsync(string id, CancellationToken cancellationToken = default);
```

### `StartSessionHandler` — inject `ICharacterService`

## UI Changes

### `CharacterSheetComponent.razor`
Add an optional `Stats` parameter:
```csharp
[Parameter] public IReadOnlyDictionary<string, int>? Stats { get; set; }
```
Render field value as:
```razor
@(Stats != null && Stats.TryGetValue(field.Key, out var v) ? v : field.DefaultValue)
```

### `SessionComponent.razor`
Pass stats when rendering the sheet:
```razor
<CharacterSheetComponent Schema="_sessionState.CharacterSheetSchema"
                          Stats="_sessionState.Stats"
                          Theme="@ThemeService.CurrentTheme" />
```

### `CharacterDetailComponent.razor`
Load stats on init and pass them:
```csharp
private IReadOnlyDictionary<string, int>? _stats;

// in OnParametersSetAsync, after loading _character:
_stats = await CharacterClient.GetStatsAsync(CharacterId);
```
```razor
<CharacterSheetComponent Schema="_schema" Stats="_stats" />
```

### `NewSessionComponent.razor`
Pass `character.Id` in the request:
```csharp
var request = new StartSessionRequest(character.Id, character.EntityId ?? "Unknown", character.GameId ?? GameSystemIds.DnD5e);
```

## Test Cases

### `DnD5eRuleBook.ExtractStats` (xUnit, `API.Services.Tests`)
- [ ] `ExtractStats_DataContainsAllSchemaKeys_ReturnsValueForEachField`
- [ ] `ExtractStats_DataHasPartialKeys_MissingFieldsFallBackToDefault`
- [ ] `ExtractStats_EmptyJsonObject_ReturnsAllDefaults`
- [ ] `ExtractStats_StrengthIs18_StatsContainStrength18`

### `GetCharacterStatsHandler` (xUnit, `API.Tests`)
- [ ] `GetCharacterStats_KnownCharacterId_ReturnsExtractedStats`
- [ ] `GetCharacterStats_UnknownCharacterId_ReturnsEmptyDictionary`

### `StartSessionHandler` (xUnit, `API.Tests`)
- [ ] `StartSession_WithValidCharacterId_SessionStateStatsMatchCharacterData`
- [ ] `StartSession_CharacterNotFound_StatsAreAllDefaults`

### `CharacterSheetComponent` (bUnit, `Blazor.Tests`)
- [ ] `CharacterSheet_WithStats_DisplaysRealValuesNotDefaults`
- [ ] `CharacterSheet_WithNullStats_DisplaysSchemaDefaultValues`

## Open Questions
- Should `StartSessionHandler` throw if `CharacterId` resolves to `null` (character deleted), or silently fall back to defaults? — Spec assumes silent fallback for now.
- Should `GetCharacterStatsHandler` return `404` or an empty dict for unknown characters? — Spec uses empty dict; handler returns `Ok({})` not `NotFound`.

## Implementation Notes
<!-- Updated during implementation -->
