using System.Text.Json;
using API.Services.Abstraction;
using API.Services.GameSystems.DnD5e;
using Mediator.Mediator.Contracts.Characters;
using Mediator.Mediator.Handlers.Characters;
using Models.Common;
using Models.Interfaces;

namespace DemonsAndDogs.API.Tests.Handlers;

// ---------------------------------------------------------------------------
// Fakes
// ---------------------------------------------------------------------------

file class FakeCharacterService : ICharacterService
{
    public List<CharacterResource> Characters { get; set; } = [];

    public Task<IEnumerable<CharacterResource>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IEnumerable<CharacterResource>>(Characters);

    public Task<CharacterResource?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        Task.FromResult(Characters.FirstOrDefault(c => c.Id == id));

    public Task<IEnumerable<CharacterResource>> GetBySystemIdAsync(string systemId, CancellationToken cancellationToken = default) =>
        Task.FromResult<IEnumerable<CharacterResource>>(Characters.Where(c => c.GameId == systemId).ToList());
}

file class FakeRegistry : IGameSystemRegistry
{
    public IRuleBook Get(string systemId) => new DnD5eRuleBook();
    public IEnumerable<IRuleBook> GetAll() => [new DnD5eRuleBook()];
}

// ---------------------------------------------------------------------------
// Tests
// ---------------------------------------------------------------------------

/// <summary>
/// Unit tests for character MediatR handlers.
/// GetCharacters, GetCharacter, and GetCharactersBySystem delegate to ICharacterService.
/// GetCharacterStats also uses the GameSystemRegistry to extract stats from character data.
/// </summary>
public class CharacterHandlersTests
{
    // -----------------------------------------------------------------------
    // GetCharactersHandler
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetCharacters_WithCharacters_ReturnsAll()
    {
        var service = new FakeCharacterService
        {
            Characters = [new() { Id = "ch1", EntityId = "Gimli" }, new() { Id = "ch2", EntityId = "Legolas" }]
        };
        var handler = new GetCharactersHandler(service);

        var result = await handler.Handle(new GetCharactersRequest(), default);

        Assert.Equal(2, result.Count());
    }

    // -----------------------------------------------------------------------
    // GetCharacterHandler
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetCharacter_ExistingId_ReturnsCharacter()
    {
        var service = new FakeCharacterService
        {
            Characters = [new() { Id = "ch1", EntityId = "Gimli" }]
        };
        var handler = new GetCharacterHandler(service);

        var result = await handler.Handle(new GetCharacterRequest("ch1"), default);

        Assert.NotNull(result);
        Assert.Equal("Gimli", result.EntityId);
    }

    [Fact]
    public async Task GetCharacter_UnknownId_ReturnsNull()
    {
        var handler = new GetCharacterHandler(new FakeCharacterService());

        var result = await handler.Handle(new GetCharacterRequest("unknown"), default);

        Assert.Null(result);
    }

    // -----------------------------------------------------------------------
    // GetCharactersBySystemHandler
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetCharactersBySystem_MatchingSystem_ReturnsFiltered()
    {
        var service = new FakeCharacterService
        {
            Characters =
            [
                new() { Id = "ch1", EntityId = "Gimli", GameId = "dnd5e" },
                new() { Id = "ch2", EntityId = "Legolas", GameId = "dnd5e" },
                new() { Id = "ch3", EntityId = "Other", GameId = "pathfinder" }
            ]
        };
        var handler = new GetCharactersBySystemHandler(service);

        var result = await handler.Handle(new GetCharactersBySystemRequest("dnd5e"), default);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetCharactersBySystem_UnknownSystem_ReturnsEmpty()
    {
        var handler = new GetCharactersBySystemHandler(new FakeCharacterService());

        var result = await handler.Handle(new GetCharactersBySystemRequest("unknown"), default);

        Assert.Empty(result);
    }

    // -----------------------------------------------------------------------
    // GetCharacterStatsHandler
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetCharacterStats_ValidCharacterWithData_ReturnsExtractedStats()
    {
        var service = new FakeCharacterService
        {
            Characters =
            [
                new()
                {
                    Id = "ch1",
                    EntityId = "Gimli",
                    GameId = "dnd5e",
                    Data = JsonSerializer.Deserialize<JsonElement>(
                        """{"strength":18,"dexterity":12,"constitution":16}""")
                }
            ]
        };
        var handler = new GetCharacterStatsHandler(service, new FakeRegistry());

        var result = await handler.Handle(new GetCharacterStatsRequest("ch1"), default);

        Assert.True(result.Count > 0);
        Assert.Equal(18, result["strength"]);
    }

    [Fact]
    public async Task GetCharacterStats_UnknownCharacter_ReturnsEmptyDictionary()
    {
        var handler = new GetCharacterStatsHandler(new FakeCharacterService(), new FakeRegistry());

        var result = await handler.Handle(new GetCharacterStatsRequest("unknown"), default);

        Assert.Empty(result);
    }
}
