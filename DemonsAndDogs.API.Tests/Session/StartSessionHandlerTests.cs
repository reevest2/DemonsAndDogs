using System.Text.Json;
using API.Services.Characters;
using API.Services.GameSystems;
using API.Services.Sessions;
using API.Services.GameSystems.DnD5e;
using AppConstants;
using DemonsAndDogs.API.Tests.Fakes;
using API.Services.Sessions.Contracts;
using API.Services.Sessions.Handlers;
using Models;
using Models.Common;
using Models.Interfaces;
using Xunit;

namespace DemonsAndDogs.API.Tests.Session;

public class StartSessionHandlerTests
{
    private const string DnD5eSystemId = "dnd5e";
    private const string FakeCharId = "char-1";

    private class FakeRegistry : IGameSystemRegistry
    {
        public Result<IRuleBook> Get(string systemId) => Result<IRuleBook>.Ok(new DnD5eRuleBook());
        public IEnumerable<IRuleBook> GetAll() => new[] { new DnD5eRuleBook() };
    }

    private class FakeCharacterService : ICharacterService
    {
        private readonly CharacterResource? _character;
        public FakeCharacterService(CharacterResource? character = null) => _character = character;

        public Task<IEnumerable<CharacterResource>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult(Enumerable.Empty<CharacterResource>());

        public Task<CharacterResource?> GetByIdAsync(string id, CancellationToken ct = default)
            => Task.FromResult(_character?.Id == id ? _character : null);

        public Task<IEnumerable<CharacterResource>> GetBySystemIdAsync(string systemId, CancellationToken ct = default)
            => Task.FromResult(Enumerable.Empty<CharacterResource>());
    }

    private static StartSessionHandler BuildHandler(ICharacterService? characterService = null) =>
        new(new FakeRegistry(), new SessionStore(), new NullSessionPersistence(), characterService ?? new NullCharacterService());

    [Fact]
    public async Task Handle_StartSession_CreatesSessionWithCorrectSystemId()
    {
        var request = new StartSessionRequest(FakeCharId, "Hero", DnD5eSystemId);
        var result = await BuildHandler().Handle(request, default);
        Assert.True(result.IsSuccess);
        Assert.Equal(DnD5eSystemId, result.Value!.SystemId);
    }

    [Fact]
    public async Task Handle_StartSession_CreatesSessionWithCorrectCharacterName()
    {
        var request = new StartSessionRequest(FakeCharId, "Hero", DnD5eSystemId);
        var result = await BuildHandler().Handle(request, default);
        Assert.True(result.IsSuccess);
        Assert.Equal("Hero", result.Value!.CharacterName);
    }

    [Fact]
    public async Task Handle_StartSession_HasNonEmptySessionId()
    {
        var request = new StartSessionRequest(FakeCharId, "Hero", DnD5eSystemId);
        var result = await BuildHandler().Handle(request, default);
        Assert.True(result.IsSuccess);
        Assert.False(string.IsNullOrEmpty(result.Value!.SessionId));
    }

    [Fact]
    public async Task Handle_StartSession_SchemaMatchesGameSystem()
    {
        var request = new StartSessionRequest(FakeCharId, "Hero", DnD5eSystemId);
        var result = await BuildHandler().Handle(request, default);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value!.CharacterSheetSchema);
        Assert.Equal(DnD5eSystemId, result.Value.CharacterSheetSchema.SystemId);
    }

    [Fact]
    public async Task Handle_StartSession_StoresSessionInStore()
    {
        var store = new SessionStore();
        var handler = new StartSessionHandler(new FakeRegistry(), store, new NullSessionPersistence(), new NullCharacterService());
        var request = new StartSessionRequest(FakeCharId, "Hero", DnD5eSystemId);
        var result = await handler.Handle(request, default);
        Assert.True(result.IsSuccess);
        Assert.True(store.TryGet(result.Value!.SessionId, out var stored));
        Assert.Equal(result.Value, stored);
    }

    [Fact]
    public async Task Handle_StartSession_WithValidCharacterId_SessionStateStatsMatchCharacterData()
    {
        var character = new CharacterResource
        {
            Id = FakeCharId,
            GameId = GameSystemIds.DnD5e,
            Data = JsonSerializer.Deserialize<JsonElement>(
                """{"strength":18,"dexterity":14,"constitution":16,"intelligence":10,"wisdom":12,"charisma":8,"hp":45,"ac":16}""")
        };
        var handler = BuildHandler(new FakeCharacterService(character));
        var request = new StartSessionRequest(FakeCharId, "Gimli", DnD5eSystemId);

        var result = await handler.Handle(request, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(18, result.Value!.Stats["strength"]);
        Assert.Equal(14, result.Value.Stats["dexterity"]);
        Assert.Equal(45, result.Value.Stats["hp"]);
    }

    [Fact]
    public async Task Handle_StartSession_CharacterNotFound_StatsAreAllDefaults()
    {
        var request = new StartSessionRequest("unknown-id", "Hero", DnD5eSystemId);
        var result = await BuildHandler(new NullCharacterService()).Handle(request, default);
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!.Stats);
    }
}
