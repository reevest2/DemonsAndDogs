using API.Services.GameSystems;
using API.Services.Sessions;
using API.Services.GameSystems.DnD5e;
using DemonsAndDogs.API.Tests.Fakes;
using API.Services.Sessions.Contracts;
using API.Services.Sessions.Handlers;
using Models;
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
        public Result<IRuleBook> Get(string systemId) => Result<IRuleBook>.Ok(new DnD5eRuleBook());
        public IEnumerable<IRuleBook> GetAll() => new[] { new DnD5eRuleBook() };
    }

    private SessionState CreateInitialState(string sessionId)
    {
        return new SessionState(
            sessionId,
            "Hero",
            DnD5eSystemId,
            new CharacterSheetSchema(DnD5eSystemId, new()),
            new Dictionary<string, int>(),
            new List<SessionEvent>());
    }

    [Fact]
    public async Task Handle_SkillCheckAction_ReturnsCorrectEventType()
    {
        var sessionId = Guid.NewGuid().ToString();
        var store = new SessionStore();
        store.Set(sessionId, CreateInitialState(sessionId));
        var registry = new FakeRegistry();
        var handler = new PerformActionHandler(registry, store, new NullSessionPersistence());
        var context = new SkillCheckContext("char1", "stealth", 0, 0, 10);
        var request = new PerformActionRequest(sessionId, ActionType.SkillCheck, context);

        var result = await handler.Handle(request, default);

        Assert.True(result.IsSuccess);
        var sessionEvent = result.Value!;
        Assert.Equal("SkillCheck", sessionEvent.EventType);
        Assert.NotNull(sessionEvent.CheckResult);
    }

    [Fact]
    public async Task Handle_AttackAction_ReturnsCorrectEventType()
    {
        var sessionId = Guid.NewGuid().ToString();
        var store = new SessionStore();
        store.Set(sessionId, CreateInitialState(sessionId));
        var registry = new FakeRegistry();
        var handler = new PerformActionHandler(registry, store, new NullSessionPersistence());
        var context = new AttackContext("sword", 0, 10);
        var request = new PerformActionRequest(sessionId, ActionType.Attack, null, context);

        var result = await handler.Handle(request, default);

        Assert.True(result.IsSuccess);
        var sessionEvent = result.Value!;
        Assert.Equal("Attack", sessionEvent.EventType);
        Assert.NotNull(sessionEvent.AttackResult);
    }

    [Fact]
    public async Task Handle_UnknownSessionId_ReturnsNotFound()
    {
        var store = new SessionStore();
        var registry = new FakeRegistry();
        var handler = new PerformActionHandler(registry, store, new NullSessionPersistence());
        var request = new PerformActionRequest("unknown", ActionType.SkillCheck, new SkillCheckContext("c", "s", 0, 0));

        var result = await handler.Handle(request, default);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.NotFound, result.Error!.Code);
    }
}
