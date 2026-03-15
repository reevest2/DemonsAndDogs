using System.Text.Json;
using API.Services.Characters;
using API.Services.GameSystems;
using API.Services.GameSystems.DnD5e;
using API.Services.Sessions;
using AppConstants;
using DataAccess.Abstraction;
using DemonsAndDogs.API.Tests.Fakes;
using Mediator.Mediator.Contracts.Session;
using Mediator.Mediator.Handlers.Session;
using Models.Common;
using Models.GameSystems;
using Models.Interfaces;
using Models.Session;
using Xunit;

namespace DemonsAndDogs.API.Tests.SessionPersistence;

// ---------------------------------------------------------------------------
// Fakes
// ---------------------------------------------------------------------------

/// <summary>In-memory IJsonResourceRepository for unit tests.</summary>
file class FakeRepository : IJsonResourceRepository
{
    private readonly List<JsonResource> _store = new();

    public int CreateCallCount { get; private set; }
    public int UpdateCallCount { get; private set; }

    public Task<JsonResource?> GetByIdAsync(string id) =>
        Task.FromResult(_store.FirstOrDefault(r => r.Id == id));

    public Task<IEnumerable<JsonResource>> GetAllAsync() =>
        Task.FromResult<IEnumerable<JsonResource>>(_store.ToList());

    public Task<IEnumerable<JsonResource>> GetByResourceKindAsync(string resourceKind) =>
        Task.FromResult<IEnumerable<JsonResource>>(_store.Where(r => r.ResourceKind == resourceKind).ToList());

    public Task<IEnumerable<JsonResource>> QueryAsync(Func<IQueryable<JsonResource>, IQueryable<JsonResource>> query) =>
        Task.FromResult<IEnumerable<JsonResource>>(query(_store.AsQueryable()).ToList());

    public Task<JsonResource> CreateAsync(JsonResource resource)
    {
        CreateCallCount++;
        _store.Add(resource);
        return Task.FromResult(resource);
    }

    public Task<JsonResource> UpdateAsync(JsonResource resource)
    {
        UpdateCallCount++;
        var idx = _store.FindIndex(r => r.Id == resource.Id);
        if (idx < 0) throw new KeyNotFoundException(resource.Id);
        _store[idx] = resource;
        return Task.FromResult(resource);
    }

    public Task DeleteAsync(string id)
    {
        _store.RemoveAll(r => r.Id == id);
        return Task.CompletedTask;
    }
}

/// <summary>ISessionPersistence spy — records SaveAsync calls for handler tests.</summary>
file class SpySessionPersistence : ISessionPersistence
{
    public List<SessionState> Saved { get; } = new();
    public SessionState? LoadResult { get; set; }
    public bool LoadWasCalled { get; private set; }

    public Task SaveAsync(SessionState state, CancellationToken ct)
    {
        Saved.Add(state);
        return Task.CompletedTask;
    }

    public Task<SessionState?> LoadAsync(string sessionId, CancellationToken ct)
    {
        LoadWasCalled = true;
        return Task.FromResult(LoadResult);
    }
}

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

file static class TestData
{
    private const string SystemId = "test-system";

    public static SessionState EmptySession(string sessionId) =>
        new(sessionId, "Hero", SystemId,
            new CharacterSheetSchema(SystemId, []),
            new Dictionary<string, int>(),
            []);

    public static SessionState SessionWithEvents(string sessionId) =>
        new(sessionId, "Hero", SystemId,
            new CharacterSheetSchema(SystemId, []),
            new Dictionary<string, int>(),
            [new SessionEvent("SkillCheck", "Rolled stealth", DateTime.UtcNow)]);

    public static FakeGameRegistry Registry => new();

    public class FakeGameRegistry : IGameSystemRegistry
    {
        public IRuleBook Get(string systemId) => new DnD5eRuleBook();
        public IEnumerable<IRuleBook> GetAll() => [new DnD5eRuleBook()];
    }
}

// ---------------------------------------------------------------------------
// Tests
// ---------------------------------------------------------------------------

public class SessionPersistenceTests
{
    // -----------------------------------------------------------------------
    // Happy Path
    // -----------------------------------------------------------------------

    [Fact]
    public async Task SaveAsync_ValidSessionState_PersistsSessionResourceWithCorrectKind()
    {
        // Arrange
        var repo = new FakeRepository();
        var sut = new JsonSessionPersistence(repo);
        var state = TestData.EmptySession("session-1");

        // Act
        await sut.SaveAsync(state, default);

        // Assert
        var saved = (await repo.GetAllAsync()).Single();
        Assert.IsType<SessionResource>(saved);
        Assert.Equal(ResourceKinds.Session, saved.ResourceKind);
    }

    [Fact]
    public async Task SaveAsync_ValidSessionState_DataContainsSerializedEventLog()
    {
        // Arrange
        var repo = new FakeRepository();
        var sut = new JsonSessionPersistence(repo);
        var state = TestData.SessionWithEvents("session-2");

        // Act
        await sut.SaveAsync(state, default);

        // Assert
        var saved = (await repo.GetAllAsync()).Single();
        var loaded = saved.Data.Deserialize<SessionState>();
        Assert.NotNull(loaded);
        Assert.Single(loaded.EventLog);
        Assert.Equal("SkillCheck", loaded.EventLog[0].EventType);
    }

    [Fact]
    public async Task SaveAsync_ExistingSession_UpdatesExistingResourceRatherThanCreatingDuplicate()
    {
        // Arrange
        var repo = new FakeRepository();
        var sut = new JsonSessionPersistence(repo);
        var state = TestData.EmptySession("session-3");

        // Act — save twice
        await sut.SaveAsync(state, default);
        await sut.SaveAsync(state, default);

        // Assert — one create, one update, one record
        Assert.Equal(1, repo.CreateCallCount);
        Assert.Equal(1, repo.UpdateCallCount);
        Assert.Single(await repo.GetAllAsync());
    }

    [Fact]
    public async Task LoadAsync_ExistingSessionId_ReturnsRehydratedSessionStateWithEventLog()
    {
        // Arrange
        var repo = new FakeRepository();
        var sut = new JsonSessionPersistence(repo);
        var state = TestData.SessionWithEvents("session-4");
        await sut.SaveAsync(state, default);

        // Act
        var loaded = await sut.LoadAsync("session-4", default);

        // Assert
        Assert.NotNull(loaded);
        Assert.Single(loaded.EventLog);
        Assert.Equal("SkillCheck", loaded.EventLog[0].EventType);
    }

    [Fact]
    public async Task LoadAsync_ExistingSessionId_SessionStateMatchesOriginal()
    {
        // Arrange
        var repo = new FakeRepository();
        var sut = new JsonSessionPersistence(repo);
        var state = TestData.EmptySession("session-5");
        await sut.SaveAsync(state, default);

        // Act
        var loaded = await sut.LoadAsync("session-5", default);

        // Assert
        Assert.NotNull(loaded);
        Assert.Equal(state.SessionId, loaded.SessionId);
        Assert.Equal(state.CharacterName, loaded.CharacterName);
        Assert.Equal(state.SystemId, loaded.SystemId);
    }

    [Fact]
    public async Task StartSessionHandler_NewSession_SessionIsSavedToRepository()
    {
        // Arrange
        var spy = new SpySessionPersistence();
        var handler = new StartSessionHandler(TestData.Registry, new SessionStore(), spy, new NullCharacterService());

        // Act
        var state = await handler.Handle(new StartSessionRequest("char-1", "Hero", "dnd5e"), default);

        // Assert
        Assert.Single(spy.Saved);
        Assert.Equal(state.SessionId, spy.Saved[0].SessionId);
    }

    [Fact]
    public async Task PerformActionHandler_AfterAction_UpdatedSessionIsSavedToRepository()
    {
        // Arrange
        var spy = new SpySessionPersistence();
        var store = new SessionStore();
        var sessionId = "session-6";
        store.Set(sessionId, TestData.EmptySession(sessionId));
        var handler = new PerformActionHandler(TestData.Registry, store, spy);
        var request = new PerformActionRequest(sessionId, ActionType.SkillCheck,
            new SkillCheckContext("c", "stealth", 0, 0, 10));

        // Act
        await handler.Handle(request, default);

        // Assert
        Assert.Single(spy.Saved);
        Assert.Single(spy.Saved[0].EventLog);
    }

    [Fact]
    public async Task GetSessionHandler_SessionNotInMemory_LoadsFromRepositoryAndCachesInStore()
    {
        // Arrange
        var sessionId = "session-7";
        var expected = TestData.EmptySession(sessionId);
        var spy = new SpySessionPersistence { LoadResult = expected };
        var store = new SessionStore();
        var handler = new GetSessionHandler(store, spy);

        // Act
        var result = await handler.Handle(new GetSessionRequest(sessionId), default);

        // Assert
        Assert.True(spy.LoadWasCalled);
        Assert.Equal(sessionId, result.SessionId);
        Assert.True(store.TryGet(sessionId, out _)); // cached
    }

    // -----------------------------------------------------------------------
    // Edge Cases
    // -----------------------------------------------------------------------

    [Fact]
    public async Task SaveAsync_SessionWithEmptyEventLog_PersistsSuccessfully()
    {
        // Arrange
        var repo = new FakeRepository();
        var sut = new JsonSessionPersistence(repo);
        var state = TestData.EmptySession("session-8");

        // Act
        await sut.SaveAsync(state, default);

        // Assert
        Assert.Equal(1, repo.CreateCallCount);
        Assert.Single(await repo.GetAllAsync());
    }

    [Fact]
    public async Task LoadAsync_SessionWithEmptyEventLog_ReturnsSessionStateWithEmptyList()
    {
        // Arrange
        var repo = new FakeRepository();
        var sut = new JsonSessionPersistence(repo);
        var state = TestData.EmptySession("session-9");
        await sut.SaveAsync(state, default);

        // Act
        var loaded = await sut.LoadAsync("session-9", default);

        // Assert
        Assert.NotNull(loaded);
        Assert.Empty(loaded.EventLog);
    }

    [Fact]
    public async Task GetSessionHandler_SessionAlreadyInMemory_DoesNotCallRepository()
    {
        // Arrange
        var sessionId = "session-10";
        var spy = new SpySessionPersistence();
        var store = new SessionStore();
        store.Set(sessionId, TestData.EmptySession(sessionId));
        var handler = new GetSessionHandler(store, spy);

        // Act
        await handler.Handle(new GetSessionRequest(sessionId), default);

        // Assert
        Assert.False(spy.LoadWasCalled);
    }

    // -----------------------------------------------------------------------
    // Error Cases
    // -----------------------------------------------------------------------

    [Fact]
    public async Task LoadAsync_UnknownSessionId_ReturnsNull()
    {
        // Arrange
        var repo = new FakeRepository();
        var sut = new JsonSessionPersistence(repo);

        // Act
        var result = await sut.LoadAsync("does-not-exist", default);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetSessionHandler_SessionNotInMemoryOrRepository_ThrowsOrReturnsNotFound()
    {
        // Arrange
        var spy = new SpySessionPersistence { LoadResult = null };
        var store = new SessionStore();
        var handler = new GetSessionHandler(store, spy);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new GetSessionRequest("missing"), default));
    }
}
