# Demons and Dogs — Claude Code Guidelines

## Rules

- No code without a spec in `docs/features/` — if none exists, create one with `scripts/New-Spec.ps1`
- No magic strings — all constants belong in `AppConstants/`
- Tests before implementation — test cases in the spec drive TDD; write tests first
- Controllers are one-liners: `return Ok(await mediator.Send(request, ct))`
- Business logic in `API.Services` only — never in controllers or Blazor pages
- Blazor components belong in `UIComponents/` — pages in `DemonsAndDogs/` reference them
- All async methods accept `CancellationToken`
- Use records for all DTOs and domain models — no classes for data transfer
- Services injected by interface only — never by concrete type

## Before Starting Any Feature

1. Call `GetStarted` (MCP tool) — loads roadmap + doc index in one call
2. Read the relevant spec in `docs/features/`
3. If no spec exists: `scripts\New-Spec.ps1 -Name "feature-name" -Title "Feature Title" -Milestone "Milestone X"`

## Before Finishing Any Task

- `dotnet build` — must pass with 0 errors
- `dotnet test` — all tests must be green
- Update `docs/state/roadmap.md` — mark completed items as Done
- Update `docs/state/current-state.md` — reflect the new state

## Key Scripts

| Script | Purpose |
|---|---|
| `scripts/New-Spec.ps1` | Scaffold a feature spec + register in index and roadmap |
| `scripts/Run-Tests.ps1` | Run tests with a clean summary |
| `scripts/Validate-Docs.ps1` | Check all docs are indexed and links are valid |
| `scripts/Check-MagicStrings.ps1` | Lint for string literals that should be AppConstants |

## Architecture Quick Reference

- **MediatR pattern**: requests are records in `Mediator/Contracts/`, handlers in `Mediator/Handlers/`, one handler per request
- **Game systems**: implement `IRuleBook`, decorate with `[GameSystem("id")]`, auto-discovered by `GameSystemRegistry`
- **Session storage**: `ISessionStore` singleton — inject it, never access `SessionStore` directly
- **Narration**: `INarrator` abstraction — current impl is `LocalLlmNarrator` (LM Studio)
- **Data model**: all persisted resources are `JsonResource` subtypes stored in one table
