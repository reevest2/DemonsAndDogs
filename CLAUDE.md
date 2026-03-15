# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Demons and Dogs** is a tabletop RPG campaign management and narration system. It is a full-stack .NET 10 application with a Blazor WebAssembly frontend, ASP.NET Core REST API backend, PostgreSQL database, and local LLM integration via LM Studio for AI narration.

## Workflow

Start every session with `/session` to check build/test health and current state.

| Command | Purpose |
|---------|---------|
| `/session` | Resume — reads state, health-checks, reports next action |
| `/ship` | Final quality gate → PR to develop → update memory |

**Branch naming:** `feature/{name}` off `develop`.
**Commits:** Conventional (`feat:`, `fix:`, `test:`, `refactor:`, `docs:`, `chore:`).
**Context management:** Use subagents for exploration. Run `/clear` between unrelated tasks.

## Git Rules

- **Never commit directly to `master` or `develop`.** Always create a feature branch first.
- **Branch base:** `feature/{name}` off `develop` (not master).
- **PR target:** Always PR to `develop` using `gh pr create --base develop`, never to `master`.
- **Before every commit:** Run `dotnet test DemonsAndDogs.sln` — do not commit if any test fails.
- **Commit messages:** Conventional commits only.
  - Format: `type: short description`
  - Types: `feat:`, `fix:`, `test:`, `refactor:`, `docs:`, `chore:`
  - No WIP commits. Each commit should be a coherent, passing unit of work.
- **PR creation:** When feature is complete (all tests green), push branch and create PR to `develop`.

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
```

## Architecture

The solution is structured as a layered monorepo with the following key projects:

### Frontend
- **`DemonsAndDogs.Builder/`** — Blazor WASM app for campaign management: campaigns, characters, documents, game systems. Runs on port 5150.
- **`DemonsAndDogs.Player/`** — Blazor WASM app for session play: start sessions, perform actions, narration. Runs on port 5160.
- **`UIComponents/`** — Shared Razor component library using Radzen.Blazor. Referenced by both frontend apps.

### API
- **`API/`** — ASP.NET Core REST API. Controllers for Campaign, Character, GameSystem, Narration, and Session. Delegates to `API.Services` via MediatR.
- **`API.Configuration/`** — DI registration for all services and infrastructure.

### Business Logic
- **`API.Services/`** — Core game logic: campaign/character/session management, game system implementations (D&D 5e), and `LocalLlmNarrator` for AI narration via LM Studio.
- **`API.Services.Mock/`** — Mock implementations used in tests and dev without a database.
- **`Mediator/`** — MediatR command/query handlers and contracts.

### Data
- **`DataAccess/`** — Entity Framework Core + PostgreSQL with pgvector. Uses a JSON document storage model: all game resources inherit from `JsonResource` and store their payload as JSONB. The `DbSeeder` populates initial game system data.
- **`API.Client/`** — Typed HTTP client abstractions used by the Blazor frontend.

### Shared
- **`Models/`** — Domain models shared across layers (resources, game systems, narration interfaces).
- **`AppConstants/`** — Global constants for GameSystemIds, ResourceKinds, ActionEventTypes, etc.

### Data Model Pattern

All persisted entities extend `JsonResource` with fields: `Id`, `EntityId`, `GameId`, `Kind`, `CreatedAt`, `UpdatedAt`, `IsDeleted`, `Version`, and `Data` (JSONB). Concrete types include `CampaignResource`, `CharacterResource`, `SessionResource`, `DocumentResource`, etc. Soft-delete is used (`IsDeleted`).

### External Dependencies

- **PostgreSQL** at `localhost:5432`, database `demonsanddogs`, user `postgres`. Requires the `pgvector` extension.
- **LM Studio** at `http://127.0.0.1:1234/api` with model `google/gemma-3-4b` for AI narration.
- **CORS**: API allows `http://localhost:5150` (Builder) and `http://localhost:5160` (Player); both call `https://localhost:44390`.

### Testing Stack

- **xunit** for all test projects
- **bunit** for Blazor component tests (`DemonsAndDogs.Tests`)
- **NSubstitute** for mocking
- **Microsoft.AspNetCore.Mvc.Testing** for API integration tests
