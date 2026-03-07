# Game Engine

## Overview
The TTRPG game engine supports multiple rule systems through a strategy pattern.
Each game system implements `IRuleBook`, which is the core extension point.
The `GameSystemRegistry` maps system IDs to their `IRuleBook` implementations.

## Key Interfaces (all in Models/Interfaces/)

### IRuleBook
The primary extension point. Implement this to add a new game system.

```csharp
public interface IRuleBook
{
    string SystemId { get; }
    string DisplayName { get; }

    CheckResult ResolveSkillCheck(SkillCheckContext context);
    AttackResult ResolveAttack(AttackContext context);
    CharacterSheetSchema GetCharacterSheetSchema();
}
```

### INarrator
AI narration abstraction. Describes events in the game world using AI.

### ICharacterSheetFactory
Generates dynamic character sheet UI schemas based on the active game system.

## Adding a New Game System

1. Add domain models to `Models/GameSystems/{Name}/`
2. Implement `IRuleBook` in `API.Services/GameSystems/{Name}/{Name}RuleBook.cs`
3. Decorate with `[GameSystem("system-id")]`
4. Register in `API.Services/GameSystemRegistry`
5. Add MediatR contracts to `Mediator/Contracts/GameSystems/`
6. Add handlers to `Mediator/Handlers/GameSystems/`
7. Add a spec doc to `docs/game-systems/{name}.md`

## GameSystemAttribute
```csharp
[GameSystem("dnd5e")]
public class DnD5eRuleBook : IRuleBook { ... }
```
The registry uses reflection to discover all decorated implementations.

## Game Systems
| ID | Name | Status |
|---|---|---|
| _(none yet)_ | | |

## MediatR Flow
```
Blazor page
  → API.Client.ResolveSkillCheckAsync(request)
    → API controller
      → _mediator.Send(new ResolveSkillCheckRequest(systemId, context))
        → ResolveSkillCheckHandler
          → GameSystemRegistry.Get(systemId).ResolveSkillCheck(context)
            → {Name}RuleBook.ResolveSkillCheck(context)
```
