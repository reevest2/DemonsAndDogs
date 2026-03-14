using System.Net;
using System.Net.Http.Json;
using DemonsAndDogs.API.Tests.Startup;
using Models.GameSystems;

namespace DemonsAndDogs.API.Tests.Controllers;

/// <summary>
/// Integration tests for the GameSystem API endpoints.
/// Game systems are discovered via reflection — DnD5e is always registered.
/// </summary>
public class GameSystemControllerTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client;

    public GameSystemControllerTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // -----------------------------------------------------------------------
    // GET /api/gamesystem
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/gamesystem");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_IncludesDnD5e()
    {
        var systems = await _client.GetFromJsonAsync<List<GameSystemDescriptor>>("/api/gamesystem");

        Assert.NotNull(systems);
        Assert.Contains(systems, s => s.SystemId == "dnd5e");
    }

    // -----------------------------------------------------------------------
    // GET /api/gamesystem/{systemId}/schema
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetSchema_DnD5e_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/gamesystem/dnd5e/schema");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetSchema_DnD5e_ReturnsSchemaWithSections()
    {
        var schema = await _client.GetFromJsonAsync<CharacterSheetSchema>("/api/gamesystem/dnd5e/schema");

        Assert.NotNull(schema);
        Assert.Equal("dnd5e", schema.SystemId);
        Assert.NotEmpty(schema.Sections);
    }
}
