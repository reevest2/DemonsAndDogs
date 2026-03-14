using System.Net;
using System.Net.Http.Json;
using DemonsAndDogs.API.Tests.Startup;
using Models.Common;

namespace DemonsAndDogs.API.Tests.Controllers;

/// <summary>
/// Integration tests for the Character API endpoints.
/// The DbSeeder pre-populates two characters:
///   - Gimli (Dwarf Fighter, D&D 5e, 45 HP, AC 16)
///   - Legolas (Elf Ranger, D&D 5e, 38 HP, AC 14)
/// </summary>
public class CharacterControllerTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CharacterControllerTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // -----------------------------------------------------------------------
    // GET /api/character
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/character");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_ReturnsSeededCharacters()
    {
        var characters = await _client.GetFromJsonAsync<List<CharacterResource>>("/api/character");

        Assert.NotNull(characters);
        Assert.True(characters.Count >= 2);
        Assert.Contains(characters, c => c.EntityId == "Gimli");
        Assert.Contains(characters, c => c.EntityId == "Legolas");
    }

    // -----------------------------------------------------------------------
    // GET /api/character/{id}
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetById_ExistingCharacter_ReturnsCharacter()
    {
        var characters = await _client.GetFromJsonAsync<List<CharacterResource>>("/api/character");
        var gimli = characters!.First(c => c.EntityId == "Gimli");

        var character = await _client.GetFromJsonAsync<CharacterResource>($"/api/character/{gimli.Id}");

        Assert.NotNull(character);
        Assert.Equal("Gimli", character.EntityId);
    }

    [Fact]
    public async Task GetById_UnknownId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/character/does-not-exist");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // -----------------------------------------------------------------------
    // GET /api/character/system/{systemId}
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetBySystem_DnD5e_ReturnsBothSeededCharacters()
    {
        var characters = await _client.GetFromJsonAsync<List<CharacterResource>>("/api/character/system/dnd5e");

        Assert.NotNull(characters);
        Assert.True(characters.Count >= 2);
    }

    [Fact]
    public async Task GetBySystem_UnknownSystem_ReturnsEmptyList()
    {
        var characters = await _client.GetFromJsonAsync<List<CharacterResource>>("/api/character/system/unknown-system");

        Assert.NotNull(characters);
        Assert.Empty(characters);
    }

    // -----------------------------------------------------------------------
    // GET /api/character/{id}/stats
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetStats_ExistingCharacter_ReturnsStatsDictionary()
    {
        var characters = await _client.GetFromJsonAsync<List<CharacterResource>>("/api/character");
        var gimli = characters!.First(c => c.EntityId == "Gimli");

        var stats = await _client.GetFromJsonAsync<Dictionary<string, int>>($"/api/character/{gimli.Id}/stats");

        Assert.NotNull(stats);
        // Gimli is seeded with D&D 5e stats — should have ability scores
        Assert.True(stats.Count > 0);
    }

    [Fact]
    public async Task GetStats_UnknownCharacter_ReturnsEmptyDictionary()
    {
        var stats = await _client.GetFromJsonAsync<Dictionary<string, int>>("/api/character/does-not-exist/stats");

        Assert.NotNull(stats);
        Assert.Empty(stats);
    }
}
