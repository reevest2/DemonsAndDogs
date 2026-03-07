using API.Services.Abstraction;
using API.Services.GameSystems.DnD5e;
using Mediator.Mediator.Contracts.Session;
using Mediator.Mediator.Handlers.Session;
using Models.GameSystems;
using Models.Interfaces;
using Models.Session;
using Xunit;

namespace DemonsAndDogs.API.Tests.Session;

public class PerformActionHandlerTests
{
    private const string DnD5eSystemId = "dnd5e";

    private class FakeRegistry : IGameSystemRegistry
    {
        public IRuleBook Get(string systemId) => new DnD5eRuleBook();
        public IEnumerable<IRuleBook> GetAll() => new[] { new DnD5eRuleBook() };
    }

    private SessionState CreateInitialState(string sessionId)
    {
        return new SessionState(
            sessionId,
            "Hero",
            DnD5eSystemId,
            new CharacterSheetSchema(DnD5eSystemId, new()),
            new List<SessionEvent>());
    }

    [Fact]
    public async Task Handle_SkillCheckAction_ReturnsCorrectEventType()
    {
        // Arrange
        var sessionId = Guid.NewGuid().ToString();
        SessionStore.Sessions[sessionId] = CreateInitialState(sessionId);
        var registry = new FakeRegistry();
        var handler = new PerformActionHandler(registry);
        var context = new SkillCheckContext("char1", "stealth", 0, 0, 10);
        var request = new PerformActionRequest(sessionId, ActionType.SkillCheck, context);

        // Act
        var sessionEvent = await handler.Handle(request, default);

        // Assert
        Assert.Equal("SkillCheck", sessionEvent.EventType);
        Assert.NotNull(sessionEvent.CheckResult);
    }

    [Fact]
    public async Task Handle_AttackAction_ReturnsCorrectEventType()
    {
        // Arrange
        var sessionId = Guid.NewGuid().ToString();
        SessionStore.Sessions[sessionId] = CreateInitialState(sessionId);
        var registry = new FakeRegistry();
        var handler = new PerformActionHandler(registry);
        var context = new AttackContext("sword", 0, 10);
        var request = new PerformActionRequest(sessionId, ActionType.Attack, null, context);

        // Act
        var sessionEvent = await handler.Handle(request, default);

        // Assert
        Assert.Equal("Attack", sessionEvent.EventType);
        Assert.NotNull(sessionEvent.AttackResult);
    }

    [Fact]
    public async Task Handle_UnknownSessionId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var registry = new FakeRegistry();
        var handler = new PerformActionHandler(registry);
        var request = new PerformActionRequest("unknown", ActionType.SkillCheck, new SkillCheckContext("c", "s", 0, 0));

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(request, default));
    }
}
