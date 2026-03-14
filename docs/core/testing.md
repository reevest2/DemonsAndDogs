# Testing Guidelines

## Spec-First Workflow

Every feature starts with a spec, and every spec includes test cases before any code is written.

### Steps

1. **Create a spec** using `New-Spec.ps1`:
   ```powershell
   scripts\New-Spec.ps1 -Name "session-persistence" -Title "Session Persistence" -Milestone "Milestone 5 — Real Data"
   ```
   This creates `docs/features/session-persistence.md`, registers it in `docs/state/index.md`, and adds a row to `docs/state/roadmap.md`.

2. **Fill in the spec** — especially the **Test Cases** section. Test cases must be defined before any implementation begins.

3. **Write the tests** — implement the test cases from the spec in the appropriate test project (see below). Use builders from `DemonsAndDogs.API.Tests/GameSystems/Builders/` where applicable.

4. **Implement the feature** — make the tests pass.

5. **Update the roadmap** — mark the spec and feature items as `Done` in `docs/state/roadmap.md`.

The template for specs lives at `docs/templates/spec-template.md`.

---

## Test Projects

| Project | Purpose | Framework |
|---|---|---|
| `DemonsAndDogs.API.Tests` | Unit tests for API layer, handlers, services, rulebooks | xUnit |
| `DemonsAndDogs.Tests` | Unit tests for Blazor components | xUnit + bUnit |
| `DemonsAndDogs.PlaywrightTests` | End-to-end browser tests | Playwright |

## What to Test

### API Tests (highest priority)
- Game system mechanics — `DnD5eRuleBook` skill checks, attacks, natural 20/1
- `GameSystemRegistry` — reflection discovery, unknown system throws `KeyNotFoundException`
- MediatR handlers — correct delegation to services, correct response shape
- Mock services — return expected hardcoded data

### Blazor Component Tests
- Components render without errors
- Dynamic `CharacterSheetSchema` fields render correctly
- Event log displays session events in order
- Navigation links point to correct routes

### Playwright Tests (end-to-end)
- Full happy path: Home → Campaigns → New Session → Active Session
- Skill check produces a result in the event log
- Navigation works correctly across all pages

## Patterns

### Naming Convention
```
MethodName_StateUnderTest_ExpectedBehavior
ResolveSkillCheck_NaturalTwenty_ReturnsSuccess
ResolveSkillCheck_TotalBelowDC_ReturnsFailure
GameSystemRegistry_UnknownSystemId_ThrowsKeyNotFoundException
```

### API Test Structure (xUnit)
```csharp
public class DnD5eRuleBookTests
{
    private readonly DnD5eRuleBook _sut = new();

    [Fact]
    public void ResolveSkillCheck_WhenTotalMeetsDC_ReturnsSuccess()
    {
        // Arrange
        var context = new SkillCheckContext("char1", "athletics", 5, 3, 15);

        // Act
        var result = _sut.ResolveSkillCheck(context);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CheckResult>(result);
    }
}
```

### Blazor Component Test Structure (bUnit)
```csharp
public class ActionLogComponentTests : TestContext
{
    [Fact]
    public void ActionLog_WithEvents_RendersAllEvents()
    {
        // Arrange
        var events = new List<SessionEvent> { ... };

        // Act
        var cut = RenderComponent<ActionLogComponent>(
            p => p.Add(c => c.Events, events));

        // Assert
        Assert.Equal(events.Count, cut.FindAll(".event-item").Count);
    }
}
```

## What NOT to Test
- Mock service return values (they're hardcoded, testing them adds no value)
- EF Core queries (test repositories against a real test DB or use integration tests)
- Controller routing (covered by Playwright)
- Pure record constructors
