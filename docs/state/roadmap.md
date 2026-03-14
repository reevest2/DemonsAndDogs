# Roadmap

Update this doc when a feature moves to Done.

## Principles
- **Spec First**: Features are specced before any code is written.
- **Test Cases in Spec**: Every spec must include clear, reproducible test cases.
- **Tests Before Implementation**: Write unit or component tests before adding logic.
- **No Magic Strings**: All constants belong in the `AppConstants` project.
- **Always Read Relevant Docs**: Call `GetStarted` and read spec docs before coding.

## Milestone 1 — AI Context Foundation

| Feature | Status | Notes |
|---|---|---|
| `docs/index.md` | Done | Master entry point for AI tools |
| `docs/current-state.md` | Done | What's real vs mocked |
| `docs/roadmap.md` | Done | This document |
| Update `.junie/guidelines.md` | Done | Always call GetStarted before coding |
| GetStarted MCP tool | Done | Returns `index.md` + doc list in one call |
| `Sync-Docs.ps1` | Removed | Not needed — Documentation.csproj uses wildcard glob to embed docs directly from docs/ folder |

## Milestone 2 — Spec-Driven Workflow

| Feature | Status | Notes |
|---|---|---|
| `New-Spec.ps1` scaffold script | Done | Automated spec creation |
| Spec template | Done | `docs/templates/spec-template.md` |
| Update `testing.md` | Done | Spec-first workflow section added |

## Milestone 3 — TDD Foundation

| Feature | Status | Notes |
|---|---|---|
| Test data builders | Done | Builders for `SkillCheckContext`, `AttackContext`, `SessionState` |
| `DnD5eRuleBook` Unit tests | Done | Full coverage for 5e mechanics |
| Scoped `SessionStore` | Done | `ISessionStore` interface + DI singleton, all handlers injected |
| `Run-Tests.ps1` | Done | `scripts/Run-Tests.ps1` with project filter and verbose flag |
| DnD5e RuleBook Unit Tests spec | Done | See docs/features/dnd5e-rulebook-tests.md |

## Milestone 4 — Clean Code

| Feature | Status | Notes |
|---|---|---|
| Narration magic strings | Done | `ActionEventTypes`, `LmStudioApiEndpoints`, SSE constants; `DataAccess` uses `ResourceKinds` |
| `Check-MagicStrings.ps1` | Done | `scripts/Check-MagicStrings.ps1` — reports strings matching existing constants |
| CancellationToken audit | Done | Added to `INarrator`, service interfaces, mocks, all controllers |

## Milestone 5 — Real Data

| Feature | Status | Notes |
|---|---|---|
| `docs/session-persistence.md` | Planned | Persistence specification |
| Real persistence | Planned | Using hybrid JSON model from `data-model.md` |
| Flip `UseMockData` flag | Planned | Switch to real repositories |

