# Architecture

## Project Structure

The Demons and Dogs solution is organized into the following projects:

### Apps
- **DemonsAndDogs** — Blazor WASM front-end. Pages and layouts only, no business logic.
  Pages delegate to `API.Client`. Each page has a single encapsulating component.
- **UIComponents** — Reusable Razor/Blazor UI components shared across the front-end.

### API Layer
- **API** — ASP.NET Core Web API with thin controllers. One line per action: `return await _mediator.Send(request)`.
- **API.Client** — Typed HTTP client for consuming the API from Blazor. Only way Blazor talks to the backend.
- **API.Configuration** — DI registration, config binding, and startup helpers.
- **API.Services** — Business logic services. Interfaces live in `API.Services/Abstraction`.

### Infrastructure
- **Models** — Shared domain models, DTOs, contracts, interfaces, and JsonResource definitions.
- **DataAccess** — Data access layer. Interfaces in `DataAccess/Abstraction`. No DbContext outside this project.
- **AppConstants** — Shared constant values, enums, and ResourceKinds. No logic.
- **Mediator** — MediatR request contracts in `/Contracts`, handlers in `/Handlers`.
- **UIComponents** — Shared Blazor components reused across pages.
- **Documentation** — Class library containing embedded Markdown documentation files.
- **MCPServer** — MCP server that serves project documentation to AI tools on demand.

### Tests
- **DemonsAndDogs.Tests** — Unit tests for the Blazor front-end.
- **DemonsAndDogs.API.Tests** — Unit tests for the API layer.
- **DemonsAndDogs.PlaywrightTests** — End-to-end browser tests using Playwright.

## Key Patterns

### Mediator Pattern
Requests are defined as records in `Mediator/Contracts` and handled by corresponding
handlers in `Mediator/Handlers`. Controllers only call `_mediator.Send()`.
This decouples request initiation from handling and keeps controllers thin.

### Service Abstraction
API services define interfaces in `Abstraction` folders, allowing for dependency
injection and testability. Never inject concrete types.

### Records for DTOs
Records are used for all DTOs and domain models to ensure immutability and type safety.
Controllers, handlers, and services pass records between layers.

### JsonResource Pattern
All API resources are defined as `JsonResource` classes in the `Models` project.
Resource kinds are constants defined in `AppConstants/ResourceKinds.cs`.
This ensures consistent serialization and easy discoverability.

### Pages Have a Single Component
Each page in the Blazor app has a single component that encapsulates the page's
logic and UI. Parameters are defined as properties on the component.

### No Magic Strings
Avoid magic strings throughout. Use constants from the `AppConstants` project.
Resource kinds, route segments, claim types, and error codes are all constants.

### Embedded Documentation
Documentation is stored as `.md` files in `docs/` at the repo root and linked
into the `Documentation` class library as embedded assembly resources.
The `MCPServer` project exposes these via MCP tools so AI assistants can
retrieve project guidance on demand.
