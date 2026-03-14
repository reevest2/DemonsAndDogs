using System.Net;
using System.Net.Http.Json;
using DemonsAndDogs.API.Tests.Startup;
using Models.Common;

namespace DemonsAndDogs.API.Tests.Controllers;

/// <summary>
/// Integration tests for the Campaign API endpoints.
/// The DbSeeder pre-populates one campaign: "Lost Mine of Phandelver" (D&D 5e).
/// </summary>
public class CampaignControllerTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CampaignControllerTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // -----------------------------------------------------------------------
    // GET /api/campaign
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/campaign");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_ReturnsSeededCampaign()
    {
        var campaigns = await _client.GetFromJsonAsync<List<CampaignResource>>("/api/campaign");

        Assert.NotNull(campaigns);
        Assert.Contains(campaigns, c => c.EntityId == "Lost Mine of Phandelver");
    }

    // -----------------------------------------------------------------------
    // GET /api/campaign/{id}
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetById_ExistingCampaign_ReturnsOk()
    {
        // First get all to find the seeded campaign's ID
        var campaigns = await _client.GetFromJsonAsync<List<CampaignResource>>("/api/campaign");
        var seeded = campaigns!.First();

        var response = await _client.GetAsync($"/api/campaign/{seeded.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ExistingCampaign_ReturnsCampaignWithCorrectEntityId()
    {
        var campaigns = await _client.GetFromJsonAsync<List<CampaignResource>>("/api/campaign");
        var seeded = campaigns!.First();

        var campaign = await _client.GetFromJsonAsync<CampaignResource>($"/api/campaign/{seeded.Id}");

        Assert.NotNull(campaign);
        Assert.Equal("Lost Mine of Phandelver", campaign.EntityId);
    }

    [Fact]
    public async Task GetById_UnknownId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/campaign/does-not-exist");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
