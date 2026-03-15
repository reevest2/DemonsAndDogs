using AppConstants;
using API.Services.Sessions.Contracts;
using API.Services.Sessions.Handlers;
using Models.GameSystems;
using Models.Interfaces;
using Models.Narration;
using Models.Session;

namespace DemonsAndDogs.API.Tests.Handlers;

// ---------------------------------------------------------------------------
// Fakes
// ---------------------------------------------------------------------------

file class FakeNarrator : INarrator
{
    public GameEvent? LastGameEvent { get; private set; }
    public string? LastTone { get; private set; }

    public Task<NarrationResult> NarrateAsync(GameEvent gameEvent, string? tone = null, CancellationToken cancellationToken = default)
    {
        LastGameEvent = gameEvent;
        LastTone = tone;
        return Task.FromResult(new NarrationResult("The hero prevails."));
    }
}

file class FakeSessionStore : ISessionStore
{
    private readonly Dictionary<string, SessionState> _sessions = new();

    public void Set(string sessionId, SessionState state) => _sessions[sessionId] = state;

    public bool TryGet(string sessionId, out SessionState? state) =>
        _sessions.TryGetValue(sessionId, out state);
}

file static class TestData
{
    public static SessionState SessionWithSkillCheck(string sessionId) =>
        new(sessionId, "Gimli", "dnd5e",
            new CharacterSheetSchema("dnd5e", []),
            new Dictionary<string, int>(),
            [new SessionEvent("SkillCheck", "Rolled stealth", DateTime.UtcNow,
                CheckResult: new CheckResult(15, 17, true, "Success!"))]);

    public static SessionState SessionWithAttack(string sessionId) =>
        new(sessionId, "Gimli", "dnd5e",
            new CharacterSheetSchema("dnd5e", []),
            new Dictionary<string, int>(),
            [new SessionEvent("Attack", "Swung axe", DateTime.UtcNow,
                AttackResult: new AttackResult(20, 22, true, true, 12, "slashing", "Critical hit!"))]);

    public static SessionState EmptySession(string sessionId) =>
        new(sessionId, "Gimli", "dnd5e",
            new CharacterSheetSchema("dnd5e", []),
            new Dictionary<string, int>(),
            []);
}

// ---------------------------------------------------------------------------
// Tests
// ---------------------------------------------------------------------------

/// <summary>
/// Unit tests for NarrateActionHandler.
/// This handler maps the last SessionEvent to a GameEvent with metadata,
/// then delegates to INarrator for dramatic narration.
/// </summary>
public class NarrateActionHandlerTests
{
    // -----------------------------------------------------------------------
    // Happy Path
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Handle_SessionWithSkillCheck_PassesGameEventToNarrator()
    {
        var narrator = new FakeNarrator();
        var store = new FakeSessionStore();
        store.Set("s1", TestData.SessionWithSkillCheck("s1"));
        var handler = new NarrateActionHandler(narrator, store);

        await handler.Handle(new NarrateActionRequest("s1"), default);

        Assert.NotNull(narrator.LastGameEvent);
        Assert.Equal("SkillCheck", narrator.LastGameEvent.EventType);
        Assert.Equal("Gimli", narrator.LastGameEvent.SubjectId);
    }

    [Fact]
    public async Task Handle_SessionWithSkillCheck_MetadataContainsSuccessAndRoll()
    {
        var narrator = new FakeNarrator();
        var store = new FakeSessionStore();
        store.Set("s1", TestData.SessionWithSkillCheck("s1"));
        var handler = new NarrateActionHandler(narrator, store);

        await handler.Handle(new NarrateActionRequest("s1"), default);

        var metadata = narrator.LastGameEvent!.Metadata!;
        Assert.Equal("True", metadata["Success"]);
        Assert.Equal("17", metadata["Roll"]);
    }

    [Fact]
    public async Task Handle_SessionWithAttack_MetadataContainsCriticalFlag()
    {
        var narrator = new FakeNarrator();
        var store = new FakeSessionStore();
        store.Set("s1", TestData.SessionWithAttack("s1"));
        var handler = new NarrateActionHandler(narrator, store);

        await handler.Handle(new NarrateActionRequest("s1"), default);

        var metadata = narrator.LastGameEvent!.Metadata!;
        Assert.Equal("True", metadata["Success"]);
        Assert.Equal("True", metadata["Critical"]);
    }

    [Fact]
    public async Task Handle_SessionWithEvents_UsesDramaticTone()
    {
        var narrator = new FakeNarrator();
        var store = new FakeSessionStore();
        store.Set("s1", TestData.SessionWithSkillCheck("s1"));
        var handler = new NarrateActionHandler(narrator, store);

        await handler.Handle(new NarrateActionRequest("s1"), default);

        Assert.Equal(NarrationTones.Dramatic, narrator.LastTone);
    }

    // -----------------------------------------------------------------------
    // Edge Cases
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Handle_SessionWithNoEvents_ReturnsNoEventsMessage()
    {
        var narrator = new FakeNarrator();
        var store = new FakeSessionStore();
        store.Set("s1", TestData.EmptySession("s1"));
        var handler = new NarrateActionHandler(narrator, store);

        var result = await handler.Handle(new NarrateActionRequest("s1"), default);

        Assert.Equal("No events to narrate yet.", result.Text);
        Assert.Null(result.TokenStream);
    }

    // -----------------------------------------------------------------------
    // Error Cases
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Handle_UnknownSessionId_ThrowsKeyNotFoundException()
    {
        var handler = new NarrateActionHandler(new FakeNarrator(), new FakeSessionStore());

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new NarrateActionRequest("unknown"), default));
    }
}
