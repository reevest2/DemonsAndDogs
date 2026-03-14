using System.Text.Json;
using API.Services.Abstraction;
using API.Services.GameSystems.DnD5e;
using AppConstants;
using Mediator.Mediator.Contracts.Characters;
using Mediator.Mediator.Handlers.Characters;
using Models.Common;
using Models.Interfaces;
using Xunit;

namespace DemonsAndDogs.API.Tests.Characters;

// ---------------------------------------------------------------------------
// Fakes
// ---------------------------------------------------------------------------

file class FakeCharacterService : ICharacterService
{
    private readonly Dictionary<string, CharacterResource> _characters = new();

    public void Seed(CharacterResource character) => _characters[character.Id] = character;

    public Task<IEnumerable<CharacterResource>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<CharacterResource>>(_characters.Values.ToList());

    public Task<CharacterResource?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        => Task.FromResult(_characters.GetValueOrDefault(id));

    public Task<IEnumerable<CharacterResource>> GetBySystemIdAsync(string systemId, CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<CharacterResource>>(_characters.Values.Where(c => c.GameId == systemId).ToList());
}

file class FakeRegistry : IGameSystemRegistry
{
    public IRuleBook Get(string systemId) => new DnD5eRuleBook();
    public IEnumerable<IRuleBook> GetAll() => [new DnD5eRuleBook()];
}

// ---------------------------------------------------------------------------
// Tests
// ---------------------------------------------------------------------------

public class GetCharacterStatsHandlerTests
{
    [Fact]
    public async Task GetCharacterStats_KnownCharacterId_ReturnsExtractedStats()
    {
        // Arrange
        var service = new FakeCharacterService();
        service.Seed(new CharacterResource
        {
            Id = "char-1",
            GameId = GameSystemIds.DnD5e,
            Data = JsonSerializer.Deserialize<JsonElement>(
                """{"strength":18,"dexterity":14,"constitution":16,"intelligence":10,"wisdom":12,"charisma":8,"hp":45,"ac":16}""")
        });

        var handler = new GetCharacterStatsHandler(service, new FakeRegistry());

        // Act
        var stats = await handler.Handle(new GetCharacterStatsRequest("char-1"), default);

        // Assert
        Assert.Equal(18, stats["strength"]);
        Assert.Equal(14, stats["dexterity"]);
        Assert.Equal(16, stats["ac"]);
    }

    [Fact]
    public async Task GetCharacterStats_UnknownCharacterId_ReturnsEmptyDictionary()
    {
        // Arrange
        var service = new FakeCharacterService();
        var handler = new GetCharacterStatsHandler(service, new FakeRegistry());

        // Act
        var stats = await handler.Handle(new GetCharacterStatsRequest("does-not-exist"), default);

        // Assert
        Assert.Empty(stats);
    }
}
