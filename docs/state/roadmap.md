# Roadmap

Update this doc when a feature moves to Done.

## Principles
- **Spec First**: Features are specced before any code is written.
- **Test Cases in Spec**: Every spec must include clear, reproducible test cases.
- **Tests Before Implementation**: Write unit or component tests before adding logic.
- **No Magic Strings**: All constants belong in the `AppConstants` project.
- **Always Read Relevant Docs**: Call `GetStarted` and read spec docs before coding.

## Milestone 1 â€” AI Context Foundation

| Feature | Status | Notes |
|---|---|---|
| `docs/index.md` | Done | Master entry point for AI tools |
| `docs/current-state.md` | Done | What's real vs mocked |
| `docs/roadmap.md` | Done | This document |
| Update `.junie/guidelines.md` | Done | Always call GetStarted before coding |
| GetStarted MCP tool | Done | Returns `index.md` + doc list in one call |
| `Sync-Docs.ps1` | Removed | Not needed â€” Documentation.csproj uses wildcard glob to embed docs directly from docs/ folder |

## Milestone 2 â€” Spec-Driven Workflow

| Feature | Status | Notes |
|---|---|---|
| `New-Spec.ps1` scaffold script | Done | Automated spec creation |
| Spec template | Done | `docs/templates/spec-template.md` |
| Update `testing.md` | Done | Spec-first workflow section added |

## Milestone 3 â€” TDD Foundation

| Feature | Status | Notes |
|---|---|---|
| Test data builders | Done | Builders for `SkillCheckContext`, `AttackContext`, `SessionState` |
| `DnD5eRuleBook` Unit tests | Done | Full coverage for 5e mechanics |
| Scoped `SessionStore` | Done | `ISessionStore` interface + DI singleton, all handlers injected |
| `Run-Tests.ps1` | Done | `scripts/Run-Tests.ps1` with project filter and verbose flag |
| DnD5e RuleBook Unit Tests spec | Done | See docs/features/dnd5e-rulebook-tests.md |

## Milestone 4 â€” Clean Code

| Feature | Status | Notes |
|---|---|---|
| Narration magic strings | Done | `ActionEventTypes`, `LmStudioApiEndpoints`, SSE constants; `DataAccess` uses `ResourceKinds` |
| `Check-MagicStrings.ps1` | Done | `scripts/Check-MagicStrings.ps1` â€” reports strings matching existing constants |
| CancellationToken audit | Done | Added to `INarrator`, service interfaces, mocks, all controllers |

## Milestone 5 â€” Real Data

| Feature | Status | Notes |
|---|---|---|
| `docs/session-persistence.md` | Done | Persistence specification |
| Session Persistence | Done | ISessionPersistence + JsonSessionPersistence + handler wiring |
| Real Campaign and Character Services | Done | JsonCampaignService + JsonCharacterService backed by IJsonResourceRepository |
| Flip `UseMockData` flag | Done | UseMockData=false; DbSeeder seeds 1 campaign + 2 characters on startup; GameSystemIds constant added |

## Milestone 6 â€" Character Stats

| Feature | Status | Notes |
|---|---|---|
| Real Character Stats | Done | ExtractStats on IRuleBook, Stats on SessionState, GET /character/{id}/stats, CharacterSheetComponent renders real values |
