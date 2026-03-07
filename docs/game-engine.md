# Game Engine

## Overview
The TTRPG game engine supports multiple rule systems through a strategy pattern.
Each game system implements `IRuleBook`, which is the core extension point.
The `GameSystemRegistry` (implementing `IGameSystemRegistry`) automatically discovers these implementations via reflection.

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

## Core Records (in Models/GameSystems/)

### Skill Check
```csharp
public record SkillCheckContext(
    string CharacterId,
    string SkillId, 
    int AbilityModifier, 
    int ProficiencyBonus, 
    int? DifficultyClass = null, 
    Dictionary<string, int>? AdditionalModifiers = null);

public record CheckResult(
    int RollValue, 
    int TotalResult, 
    bool IsSuccess, 
    string Message);
```

### Attack
```csharp
public record AttackContext(
    string WeaponId,
    int AttackModifier,
    int? TargetArmorClass = null,
    Dictionary<string, int>? AdditionalModifiers = null);

public record AttackResult(
    int AttackRoll,
    int TotalAttackResult,
    bool IsHit,
    bool IsCriticalHit,
    int? DamageDealt = null,
    string? DamageType = null,
    string Message = "");
```

## Adding a New Game System

1. Add domain models to `Models/GameSystems/{Name}/`
2. Implement `IRuleBook` in `API.Services/GameSystems/{Name}/{Name}RuleBook.cs`
3. Decorate with `[GameSystem("system-id")]`
4. Add MediatR contracts to `Mediator/Contracts/GameSystems/`
5. Add handlers to `Mediator/Handlers/GameSystems/`
6. Add a spec doc to `docs/game-systems/{name}.md`

*Note: `GameSystemRegistry` automatically discovers all `[GameSystem]` decorated classes in the `API.Services` assembly.*

## GameSystemAttribute
```csharp
[GameSystem("dnd5e")]
public class DnD5eRuleBook : IRuleBook { ... }
```

## Game Systems
| ID | Name | Status |
|---|---|---|
| `dnd5e` | Dungeons & Dragons 5th Edition | Implemented |

## MediatR Flow
```
Blazor page
  → API.Client.ResolveSkillCheckAsync(request)
    → API controller
      → _mediator.Send(new ResolveSkillCheckRequest(systemId, context))
        → ResolveSkillCheckHandler
          → IGameSystemRegistry.Get(systemId).ResolveSkillCheck(context)
            → {Name}RuleBook.ResolveSkillCheck(context)
```
