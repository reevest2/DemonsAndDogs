# Demons and Dogs — Master Index

Demons and Dogs is a high-performance, multi-system TTRPG engine built with Blazor WASM and .NET 10. It features AI-powered narration and a flexible game system architecture that allows for rapid integration of different rulebooks like D&D 5e, Call of Cthulhu, and Pathfinder. The core goal is to provide a unified platform for digital TTRPG sessions with rich, dynamic character sheets and immersive AI storytelling.

## Read This First

Before writing any code or starting a new feature, AI tools **must**:
1.  **Call `GetStarted`** from the MCP server to initialize context and see the current project state.
2.  **Read the relevant spec doc** in `docs/` for any feature you are working on. Specs drive implementation.
3.  **Identify patterns** in `docs/architecture.md` and `docs/best-practices.md` before adding new logic.

## Project Structure

The solution follows a clean architecture with MediatR-driven communication and clear separation of concerns:

- **Apps/**
    - **DemonsAndDogs**: Blazor WASM frontend. Contains pages, layouts, and the entry point. Delegates all logic to `API.Client`.
- **API/**
    - **API**: Thin ASP.NET Core controllers that send MediatR requests.
    - **API.Client**: Typed HTTP client for Blazor-to-API communication.
    - **API.Configuration**: Dependency injection registration and startup configuration.
    - **API.Services**: Business logic services and `IRuleBook` implementations.
- **Infrastructure/**
    - **Models**: Shared records for DTOs, domain models, and `JsonResource` definitions.
    - **DataAccess**: EF Core repositories and abstractions for data persistence.
    - **AppConstants**: Shared constants, enums, and resource kinds. No business logic.
    - **Mediator**: MediatR contracts (Requests/Responses) and their corresponding Handlers.
    - **UIComponents**: Shared Razor components used by the Blazor WASM frontend.
    - **Documentation**: Embedded Markdown files served via assembly resources.
    - **MCPServer**: MCP server exposing project documentation and state to AI tools.
- **Tests/**: Various test projects using xUnit, bUnit, and Playwright.

For more details, see [architecture.md](architecture.md).

## Documentation Index

| Document | Description |
|---|---|
| [architecture.md](architecture.md) | High-level system design, project structure, and key patterns. |
| [best-practices.md](best-practices.md) | Coding standards, Blazor patterns, and TTRPG-specific rules. |
| [data-model.md](data-model.md) | Hybrid relational/JSON schema for game system flexibility. |
| [game-engine.md](game-engine.md) | Game system extension points, `IRuleBook`, and system registry. |
| [game-session.md](game-session.md) | Session state management, event logging, and game flow. |
| [narration-lmstudio.md](narration-lmstudio.md) | LM Studio local AI narration setup and troubleshooting. |
| [roadmap.md](roadmap.md) | Feature roadmap with milestone status tracking. |
| [testing.md](testing.md) | Test project map, naming conventions, and bUnit/xUnit patterns. |
| [ui-themes.md](ui-themes.md) | Design system, themes, and CSS architecture. |

## Current State

For the authoritative source on what features are currently implemented, what is mocked, and what's next on the roadmap, see [current-state.md](current-state.md).

## Working With AI

When working on this project, AI assistants must adhere to these rules:
- **Spec-Driven Development**: Always read the relevant spec doc before writing code.
- **No Hardcoded Values**: Never hardcode values that belong in `AppConstants`.
- **Follow Patterns**: Adhere to the architectural patterns defined in `best-practices.md`.
- **Test First**: Write spec-driven test cases (xUnit or bUnit) before starting the implementation.
- **Clean Controllers**: Controllers must remain one-line MediatR senders.
- **Records Only**: Use C# records for all data transfer and domain models.
