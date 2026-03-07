using API.Services.Abstraction;
using API.Services.GameSystems.DnD5e;
using Mediator.Mediator.Contracts.Session;
using Mediator.Mediator.Handlers.Session;
using Models.Interfaces;
using Xunit;

namespace DemonsAndDogs.API.Tests.Session;

public class StartSessionHandlerTests
{
    private const string DnD5eSystemId = "dnd5e";

    private class FakeRegistry : IGameSystemRegistry
    {
        public IRuleBook Get(string systemId) => new DnD5eRuleBook();
        public IEnumerable<IRuleBook> GetAll() => new[] { new DnD5eRuleBook() };
    }

    [Fact]
    public async Task Handle_StartSession_CreatesSessionWithCorrectSystemId()
    {
        // Arrange
        var registry = new FakeRegistry();
        var handler = new StartSessionHandler(registry);
        var request = new StartSessionRequest("Hero", DnD5eSystemId);

        // Act
        var state = await handler.Handle(request, default);

        // Assert
        Assert.Equal(DnD5eSystemId, state.SystemId);
    }

    [Fact]
    public async Task Handle_StartSession_CreatesSessionWithCorrectCharacterName()
    {
        // Arrange
        var registry = new FakeRegistry();
        var handler = new StartSessionHandler(registry);
        var request = new StartSessionRequest("Hero", DnD5eSystemId);

        // Act
        var state = await handler.Handle(request, default);

        // Assert
        Assert.Equal("Hero", state.CharacterName);
    }

    [Fact]
    public async Task Handle_StartSession_HasNonEmptySessionId()
    {
        // Arrange
        var registry = new FakeRegistry();
        var handler = new StartSessionHandler(registry);
        var request = new StartSessionRequest("Hero", DnD5eSystemId);

        // Act
        var state = await handler.Handle(request, default);

        // Assert
        Assert.False(string.IsNullOrEmpty(state.SessionId));
    }

    [Fact]
    public async Task Handle_StartSession_SchemaMatchesGameSystem()
    {
        // Arrange
        var registry = new FakeRegistry();
        var handler = new StartSessionHandler(registry);
        var request = new StartSessionRequest("Hero", DnD5eSystemId);

        // Act
        var state = await handler.Handle(request, default);

        // Assert
        Assert.NotNull(state.CharacterSheetSchema);
        Assert.Equal(DnD5eSystemId, state.CharacterSheetSchema.SystemId);
    }
}
