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
| `New-Spec.ps1` scaffold script | In Progress | Automated spec creation |
| Spec template | Planned | Template with required sections including Test Cases |
| Update `testing.md` | Planned | Document spec-first workflow |

## Milestone 3 — TDD Foundation

| Feature | Status | Notes |
|---|---|---|
| Test data builders | Planned | Builders for `SkillCheckContext`, `AttackContext`, `SessionState` |
| `DnD5eRuleBook` Unit tests | Planned | Full coverage for 5e mechanics |
| Scoped `SessionStore` | Planned | Replace static `SessionStore` with scoped service |
| `Run-Tests.ps1` | Planned | Rider External Tool for quick test runs |

## Milestone 4 — Clean Code

| Feature | Status | Notes |
|---|---|---|
| Narration magic strings | Planned | Finish magic strings in narration logic |
| `Check-MagicStrings.ps1` | Planned | Linting tool for magic strings |
| CancellationToken audit | Planned | Ensure all async methods are cancelable |

## Milestone 5 — Real Data

| Feature | Status | Notes |
|---|---|---|
| `docs/session-persistence.md` | Planned | Persistence specification |
| Real persistence | Planned | Using hybrid JSON model from `data-model.md` |
| Flip `UseMockData` flag | Planned | Switch to real repositories |
