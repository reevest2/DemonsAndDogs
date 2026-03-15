# CLAUDE.md

## Project Overview

**Demons and Dogs** is a tabletop RPG campaign management and narration system. Full-stack .NET 10: Blazor WASM frontend, ASP.NET Core REST API, PostgreSQL + pgvector, and local LLM narration via LM Studio.

## Workflow

Start every session with `/session` to check build/test health and current state.

| Skill | Purpose |
|-------|---------|
| `/session` | Resume — reads state, health-checks, reports next action |
| `/ship` | Final quality gate → PR to develop → update memory |

**Context management:** Use subagents for exploration. Run `/clear` between unrelated tasks.

## Git Rules

- **Never commit directly to `master` or `develop`.** Always create a feature branch first.
- **Branch naming:** `feature/{name}` off `develop` (not master).
- **PR target:** Always `gh pr create --base develop`, never to `master`.
- **Before every commit:** Run `dotnet test DemonsAndDogs.sln` — do not commit if any test fails.
- **Commit messages:** Conventional commits only.
  - Format: `type: short description`
  - Types: `feat:`, `fix:`, `test:`, `refactor:`, `docs:`, `chore:`
  - No WIP commits. Each commit should be a coherent, passing unit of work.

## Commands

### Build

```bash
dotnet build DemonsAndDogs.sln
```

### Run

```bash
# API server (https://localhost:44390)
dotnet run --project API/API.csproj

# Builder app — campaign management (http://localhost:5150)
dotnet run --project DemonsAndDogs.Builder/DemonsAndDogs.Builder.csproj

# Player app — session play (http://localhost:5160)
dotnet run --project DemonsAndDogs.Player/DemonsAndDogs.Player.csproj
```

### Test

```bash
# All tests
dotnet test DemonsAndDogs.sln

# Frontend/component tests only
dotnet test DemonsAndDogs.Tests/DemonsAndDogs.Tests.csproj

# API/integration tests only
dotnet test DemonsAndDogs.API.Tests/DemonsAndDogs.API.Tests.csproj

# E2E tests (Playwright)
dotnet test DemonsAndDogs.E2E.Tests/DemonsAndDogs.E2E.Tests.csproj
```

### Scripts

```bash
# Create feature branch off develop
scripts/claude-start.ps1 -FeatureName "my-feature"

# Sync local develop with origin
scripts/sync-develop.ps1
```

## Architecture

| Project | Purpose |
|---------|---------|
| `DemonsAndDogs.Builder/` | Blazor WASM — campaign management (port 5150) |
| `DemonsAndDogs.Player/` | Blazor WASM — session play (port 5160) |
| `UIComponents/` | Shared Razor component library (Radzen.Blazor) |
| `API/` | ASP.NET Core REST API — delegates to services via MediatR |
| `API.Configuration/` | DI registration for all services and infrastructure |
| `API.Services/` | Core game logic, game system implementations, LLM narration |
| `API.Services.Mock/` | Mock implementations for tests and dev without DB |
| `Mediator/` | MediatR command/query handlers and contracts |
| `DataAccess/` | EF Core + PostgreSQL with pgvector, JSON document storage |
| `API.Client/` | Typed HTTP client abstractions for Blazor frontends |
| `Models/` | Domain models shared across layers |
| `AppConstants/` | Global constants (GameSystemIds, ResourceKinds, etc.) |

### Data Model Pattern

All persisted entities extend `JsonResource` with fields: `Id`, `EntityId`, `GameId`, `Kind`, `CreatedAt`, `UpdatedAt`, `IsDeleted`, `Version`, and `Data` (JSONB). Concrete types include `CampaignResource`, `CharacterResource`, `SessionResource`, `DocumentResource`, etc. Soft-delete via `IsDeleted`.

### External Dependencies

- **PostgreSQL** at `localhost:5432`, database `demonsanddogs`, user `postgres`. Requires `pgvector` extension.
- **LM Studio** at `http://127.0.0.1:1234/api` with model `google/gemma-3-4b`.
- **CORS**: API allows `http://localhost:5150` and `http://localhost:5160`; both call `https://localhost:44390`.
