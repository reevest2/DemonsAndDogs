# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Demons and Dogs** is a tabletop RPG campaign management and narration system. It is a full-stack .NET 10 application with a Blazor WebAssembly frontend, ASP.NET Core REST API backend, PostgreSQL database, and local LLM integration via LM Studio for AI narration.

## Workflow

This project uses spec-driven TDD, managed entirely by Claude. Start every session with `/session`.

| Command | Purpose |
|---------|---------|
| `/session` | Resume — reads state, health-checks, reports next action |
| `/new-feature [hint]` | Interview → write spec → create branch |
| `/implement` | TDD: failing tests → implement → refactor → commit |
| `/ship` | Final quality gate → PR → update memory |

**Specs:** `docs/specs/{feature}.md` — source of truth for each feature.
**Branches:** `feature/{spec-name}` off `master`.
**Commits:** Conventional (`feat:`, `fix:`, `spec:`, `test:`, `refactor:`).
**Context management:** Use subagents for exploration. Run `/clear` between unrelated tasks.

## Commands

### Build

```bash
dotnet build DemonsAndDogs.sln
```

### Run

```bash
# API server (https://localhost:44390)
dotnet run --project API/API.csproj

# Blazor WebAssembly frontend (http://localhost:5150)
dotnet run --project DemonsAndDogs/DemonsAndDogs.csproj
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
- **`DemonsAndDogs/`** — Blazor WebAssembly app. Pages, layout, and services. Calls the API via typed HTTP clients from `API.Client`.
- **`UIComponents/`** — Shared Razor component library using Radzen.Blazor.

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
- **CORS**: API allows `http://localhost:5150`; frontend calls `https://localhost:44390`.

### Testing Stack

- **xunit** for all test projects
- **bunit** for Blazor component tests (`DemonsAndDogs.Tests`)
- **NSubstitute** for mocking
- **Microsoft.AspNetCore.Mvc.Testing** for API integration tests
