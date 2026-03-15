using System.Text.Json;
using API.Controllers;
using API.Services.Characters;
using API.Services.GameSystems;
using API.Services.GameSystems.DnD5e;
using AppConstants;
using Microsoft.AspNetCore.Mvc;
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

        var controller = new CharacterController(service, new FakeRegistry());

        // Act
        var result = await controller.GetStats("char-1", CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        var stats = okResult.Value as IReadOnlyDictionary<string, int>;
        Assert.NotNull(stats);
        Assert.Equal(18, stats["strength"]);
        Assert.Equal(14, stats["dexterity"]);
        Assert.Equal(16, stats["ac"]);
    }

    [Fact]
    public async Task GetCharacterStats_UnknownCharacterId_ReturnsEmptyDictionary()
    {
        // Arrange
        var service = new FakeCharacterService();
        var controller = new CharacterController(service, new FakeRegistry());

        // Act
        var result = await controller.GetStats("does-not-exist", CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        var stats = okResult.Value as IReadOnlyDictionary<string, int>;
        Assert.NotNull(stats);
        Assert.Empty(stats);
    }
}
