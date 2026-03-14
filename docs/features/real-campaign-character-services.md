# Real Campaign and Character Services

## Overview
Replace mock `ICampaignService` and `ICharacterService` implementations with real `IJsonResourceRepository`-backed services that read `CampaignResource` and `CharacterResource` from the database.

## Goals
Eliminate mock services so that campaign and character data is sourced from the database, completing the Milestone 5 goal of real data access.

## Constraints
- No magic strings — use `ResourceKinds.Campaign` and `ResourceKinds.Character` constants
- Services injected by interface only — never `JsonCampaignService` or `JsonCharacterService` directly
- All async methods accept `CancellationToken`
- Business logic in `API.Services` only
- Use records for all return types — `CampaignResource` and `CharacterResource` are already records
- Query via `IJsonResourceRepository.QueryAsync()` only — no direct `DbContext` access
- Filter by `Kind` property, not `ResourceKind` string property

## Data Model
No new records required. Uses existing:
- `CampaignResource` — `JsonResource` subtype, `Kind == ResourceKinds.Campaign`
- `CharacterResource` — `JsonResource` subtype, `Kind == ResourceKinds.Character`

Seed data: add at least one `CampaignResource` and two `CharacterResource` records to the DB seed so the app works without manual data entry.

## Flow

### GetAll (campaigns)
1. Controller → `GetCampaignsRequest` → MediatR
2. Handler calls `ICampaignService.GetAllAsync(ct)`
3. `JsonCampaignService` calls `repository.QueryAsync(q => q.OfType<CampaignResource>())`
4. Returns `IEnumerable<CampaignResource>`

### GetById (campaigns / characters)
1. Controller → `GetCampaignByIdRequest(id)` → MediatR
2. Handler calls `ICampaignService.GetByIdAsync(id, ct)`
3. `JsonCampaignService` calls `repository.QueryAsync(q => q.OfType<CampaignResource>().Where(r => r.Id == id))`
4. Returns first match or `null`

### GetBySystemId (characters)
1. Handler calls `ICharacterService.GetBySystemIdAsync(systemId, ct)`
2. `JsonCharacterService` calls `repository.QueryAsync(q => q.OfType<CharacterResource>().Where(r => r.GameId == systemId))`

## API Changes
No new MediatR requests. Existing request/handler pairs remain unchanged — only the concrete service implementations are swapped.

New concrete types (in `API.Services/`):
- `JsonCampaignService(IJsonResourceRepository repository) : ICampaignService`
- `JsonCharacterService(IJsonResourceRepository repository) : ICharacterService`

DI registration: replace `MockCampaignService` and `MockCharacterService` in the DI container with their `Json*` counterparts.

## UI Changes
None — the Blazor components consume `ICampaignService` and `ICharacterService` through MediatR handlers; no changes required.

## Test Cases

### Happy Path
- [ ] `GetAllAsync_TwoCampaignsInRepository_ReturnsBothCampaigns`
- [ ] `GetAllAsync_TwoCharactersInRepository_ReturnsBothCharacters`
- [ ] `GetByIdAsync_ExistingCampaignId_ReturnsCampaign`
- [ ] `GetByIdAsync_ExistingCharacterId_ReturnsCharacter`
- [ ] `GetBySystemIdAsync_MatchingGameId_ReturnsOnlyCharactersForThatSystem`

### Edge Cases
- [ ] `GetAllAsync_NoCampaignsInRepository_ReturnsEmptyList`
- [ ] `GetAllAsync_NoCharactersInRepository_ReturnsEmptyList`
- [ ] `GetAllAsync_RepositoryContainsMixedKinds_ReturnsOnlyCampaigns`
- [ ] `GetAllAsync_RepositoryContainsMixedKinds_ReturnsOnlyCharacters`
- [ ] `GetBySystemIdAsync_NoCharactersMatchGameId_ReturnsEmptyList`
- [ ] `GetBySystemIdAsync_MultipleSystemsInRepo_ReturnsOnlyRequestedSystem`

### Error Cases
- [ ] `GetByIdAsync_UnknownCampaignId_ReturnsNull`
- [ ] `GetByIdAsync_UnknownCharacterId_ReturnsNull`

## Open Questions
- Should `CreateAsync` / `DeleteAsync` be added to the service interfaces now, or deferred to a later feature?
- Which seed campaigns/characters should ship with the app for local dev?

## Implementation Notes
<!-- Updated during implementation -->
