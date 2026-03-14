---
name: dotnet-patterns
description: C# and .NET coding patterns guidance. Auto-invoked when writing C# code to ensure consistency with project standards.
---

When writing or modifying C# code in this project, follow these patterns:

## Dependency Injection
- Register services via extension methods on `IServiceCollection`.
- Use constructor injection. No service locator or `IServiceProvider.GetService()` in business logic.

## Common Patterns
- **Repository Pattern**: For data access, define `IRepository<T>` interfaces in the domain layer, implement in infrastructure.
- **Result Pattern**: Return `Result<T>` for operations with expected failure cases instead of throwing exceptions. See docs/standards/error-handling.md.
- **Guard Clauses**: Use `ArgumentNullException.ThrowIfNull()` and `ArgumentException.ThrowIfNullOrEmpty()` at public method entries.

## Async/Await
- All I/O-bound operations must be async. Suffix with `Async`.
- Use `ConfigureAwait(false)` in library code (not in application entry points).
- Never use `.Result` or `.Wait()` on tasks — always `await`.

## Records and Immutability
- Use `record` types for DTOs, value objects, and immutable data.
- Use `init` properties for objects that should be immutable after construction.

## Nullable Reference Types
- This project has nullable reference types enabled. Respect nullability annotations.
- Use `?` suffix for nullable references. Don't suppress with `!` unless truly necessary.

## Always reference the full standards
Read the relevant standards files in docs/standards/ when making architectural decisions or when unsure about a convention.
