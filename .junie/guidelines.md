# DemonsAndDogs — Developer Guidelines

## Getting Started
Before writing any code: call `GetStarted` from the MCP server to load project context, then call `GetDoc` for the specific spec relevant to your task. If no spec exists for the task, stop and ask the user to create one first.

## Stack
- Blazor WASM (.NET 10), ASP.NET Core Web API, Azure
- MediatR for all request/response flows
- Records for all DTOs and domain models

## Solution Structure

```
Apps/
  DemonsAndDogs        — Blazor WASM, pages and layouts only

API/
  API                  — Thin ASP.NET Core controllers
  API.Client           — Typed HTTP client for Blazor → API calls
  API.Configuration    — DI registration, config binding, startup
  API.Services         — Business logic, interfaces in /Abstraction

Infrastructure/
  Models               — Interfaces, domain models, DTOs, JsonResources
  Mediator             — MediatR contracts in /Contracts, handlers in /Handlers
  DataAccess           — EF Core, repositories, interfaces in /Abstraction
  AppConstants         — Enums, string constants, ResourceKinds — no logic
  UIComponents         — Shared Blazor components
  Documentation        — Embedded .md docs served by MCPServer
  MCPServer            — MCP server exposing docs to AI tools

Tests/
  DemonsAndDogs.Tests
  DemonsAndDogs.API.Tests
  DemonsAndDogs.PlaywrightTests
```

## Architecture Rules

### General
- Controllers are one line: `return await _mediator.Send(request)`
- Business logic in `API.Services` handlers only
- Blazor pages use `API.Client` exclusively — never inject services directly
- All interfaces go in their project's `Abstraction` folder
- Services injected by interface only, never concrete type
- No magic strings — use constants from `AppConstants`

### Blazor WASM
- All Blazor components live in the UIComponents project, not in DemonsAndDogs. Pages in DemonsAndDogs reference components from UIComponents. The only files in DemonsAndDogs are pages, layouts, and Program.cs.

### DTOs and Models
- Use records for all DTOs and domain models — no classes for data transfer
- All API resources are `JsonResource` classes defined in `Models`
- Resource kinds are constants defined in `AppConstants/ResourceKinds.cs`

### Pages
- Each page has a single encapsulating component
- Component parameters are defined as properties on the component

### MediatR Pattern
- Requests are records defined in `Mediator/Contracts/`
- Handlers live in `Mediator/Handlers/`
- One handler per request
- Controllers send, never handle

```csharp
// Mediator/Contracts/GameSystems/ResolveSkillCheckRequest.cs
public record ResolveSkillCheckRequest(string SystemId, SkillCheckContext Context)
    : IRequest<CheckResult>;

// Mediator/Handlers/GameSystems/ResolveSkillCheckHandler.cs
public class ResolveSkillCheckHandler(GameSystemRegistry registry)
    : IRequestHandler<ResolveSkillCheckRequest, CheckResult> { ... }
```

## TTRPG Game Engine

### Key Extension Points
- `IRuleBook` in `Models/Interfaces/` — implement to add a new game system
- `GameSystemRegistry` in `API.Services/` — register new systems here
- `INarrator` in `Models/Interfaces/` — AI narration abstraction
- `ICharacterSheetFactory` in `Models/Interfaces/` — dynamic sheet generation

### How to Add a New Game System
1. Add models/contracts to `Models/GameSystems/{Name}/`
2. Implement `IRuleBook` in `API.Services/GameSystems/{Name}/{Name}RuleBook.cs`
3. Decorate with `[GameSystem("system-id")]`
4. Register in `API.Services/GameSystemRegistry`
5. Add MediatR contracts to `Mediator/Contracts/GameSystems/`
6. Add handlers to `Mediator/Handlers/GameSystems/`
7. Add a doc to `docs/game-systems/{name}.md`

## Documentation & MCP
- Docs live as `.md` files in `docs/` subfolders — organized and easy to find
- `Documentation` project embeds them as assembly resources at build time
- `MCPServer` exposes them via MCP tools to Junie on demand
- When adding a significant feature, add a corresponding doc in `docs/features/`

## Spec-Driven Development
Every feature must have a spec in `docs/features/` before implementation begins. Read the spec completely before writing any code. If the spec has a "Test Cases" section, write those tests first before writing implementation.

## No Magic Strings
All string constants must live in `AppConstants/`. Before adding any string literal, check if a constant exists. If not, add one. This applies to event type names, provider names, route segments, tone names, and SSE protocol strings.

## Before You Finish
After completing a task: check if `docs/state/current-state.md` needs updating, check if `docs/state/roadmap.md` status needs updating, run the build to confirm no errors.

## Prompt Template
Use this structure for every Junie prompt:

**Context:** [existing file or pattern to reference]
**Goal:** [what to add or change]
**Constraints:** [patterns to follow, what not to touch]
**Output:** [expected files and which projects they live in]
