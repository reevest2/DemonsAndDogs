# Testing Guidelines

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
