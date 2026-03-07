# Best Practices

## C# / .NET
- Use records for all DTOs and domain models — no classes for data transfer
- Use primary constructors for services: `public class MyService(IRepo repo)`
- Prefer `IReadOnlyList<T>` over `List<T>` for return types
- Use `sealed` on classes that aren't designed for inheritance
- Cancellation tokens on every async public method signature

## Blazor WASM
- Pages have one component — logic and UI in a single encapsulating component
- Component parameters are properties, not constructor args
- Use `API.Client` exclusively — never inject services directly into pages
- Avoid `StateHasChanged()` calls — prefer proper reactive patterns

## API
- Controllers are one line: `return await _mediator.Send(request)`
- Return `IActionResult` not concrete types from controllers
- Use `[ProducesResponseType]` attributes on all controller actions
- Validate at the handler level using FluentValidation, not in controllers

## MediatR
- Requests are records in `Mediator/Contracts/`
- One handler per request — no handler does two things
- Handlers are thin orchestrators — real logic in services
- Use notifications for cross-cutting events (not requests)

## Data Access
- Repositories abstract all EF Core — no DbContext outside DataAccess
- Use interfaces from `DataAccess/Abstraction` everywhere
- No raw SQL unless in a named query class

## TTRPG Specific
- All game rule resolution goes through IRuleBook — never hardcode rules
- No game system logic in API.Services directly — only in GameSystems/{Name}/
- Character sheet schema is always driven by IRuleBook.GetCharacterSheetSchema()