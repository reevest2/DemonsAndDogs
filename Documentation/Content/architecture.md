# Architecture

## Project Structure

The Demons and Dogs solution is organized into the following projects:

### Presentation Layer
- **DemonsAndDogs** — Blazor WASM front-end application.
- **UIComponents** — Reusable Razor/Blazor UI components shared across the front-end.

### API Layer
- **API** — ASP.NET Core Web API with controllers.
- **API.Client** — Typed HTTP client for consuming the API from Blazor.
- **API.Configuration** — API configuration and setup helpers.
- **API.Services** — Business logic services consumed by API controllers, with abstractions in `API.Services/Abstraction`.

### Domain & Data
- **Models** — Shared domain models, DTOs, contracts, and resource definitions.
- **DataAccess** — Data access layer with abstractions in `DataAccess/Abstraction`.
- **Constants** / **AppConstants** — Shared constant values used across projects.

### Infrastructure
- **Mediator** — Mediator pattern implementation with request contracts and handlers.
- **MCPServer** — MCP (Model Context Protocol) server that serves project documentation to AI tools.
- **Documentation** — Class library containing embedded Markdown documentation files.

### Tests
- **DemonsAndDogs.Tests** — Unit tests for the Blazor front-end.
- **DemonsAndDogs.API.Tests** — Unit tests for the API layer.
- **DemonsAndDogs.PlaywrightTests** — End-to-end browser tests using Playwright.

## Key Patterns

### Mediator Pattern
Requests are defined as contracts in `Mediator/Contracts` and handled by corresponding handlers in `Mediator/Handlers`. This decouples request initiation from request handling.

### Service Abstraction
API services define interfaces in `Abstraction` folders, allowing for dependency injection and testability.

### Embedded Documentation
Documentation is stored as `.md` files in `Documentation/Content` and embedded as assembly resources. The `MCPServer` project exposes these via MCP tools so AI assistants can retrieve project guidance on demand.
