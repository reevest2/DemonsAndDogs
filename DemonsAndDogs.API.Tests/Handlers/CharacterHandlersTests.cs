using System.Text.Json;
using API.Controllers;
using API.Services.Characters;
using API.Services.GameSystems;
using API.Services.GameSystems.DnD5e;
using Microsoft.AspNetCore.Mvc;
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
/// Unit tests for CharacterController.GetStats endpoint.
/// </summary>
public class CharacterHandlersTests
{
    // -----------------------------------------------------------------------
    // CharacterController.GetStats
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
        var controller = new CharacterController(service, new FakeRegistry());

        var result = await controller.GetStats("ch1", CancellationToken.None);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        var stats = okResult.Value as IReadOnlyDictionary<string, int>;
        Assert.NotNull(stats);
        Assert.True(stats.Count > 0);
        Assert.Equal(18, stats["strength"]);
    }

    [Fact]
    public async Task GetCharacterStats_UnknownCharacter_ReturnsEmptyDictionary()
    {
        var controller = new CharacterController(new FakeCharacterService(), new FakeRegistry());

        var result = await controller.GetStats("unknown", CancellationToken.None);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        var stats = okResult.Value as IReadOnlyDictionary<string, int>;
        Assert.NotNull(stats);
        Assert.Empty(stats);
    }
}
