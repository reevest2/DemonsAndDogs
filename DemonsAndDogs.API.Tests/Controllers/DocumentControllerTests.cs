using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DemonsAndDogs.API.Tests.Startup;
using Models.Common;

namespace DemonsAndDogs.API.Tests.Controllers;

/// <summary>
/// Integration tests for the Document API endpoints.
/// Documents support full CRUD: list by campaign, get by id, create, update, delete.
/// No documents are seeded — tests create their own data.
/// </summary>
public class DocumentControllerTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DocumentControllerTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private static DocumentResource MakeDocument(string campaignId = "test-campaign", string title = "Test NPC") =>
        new()
        {
            EntityId = title,
            CampaignId = campaignId,
            Data = JsonDocument.Parse("""{"category":"NPC","description":"A mysterious stranger"}""").RootElement
        };

    private async Task<DocumentResource> CreateDocumentViaApi(DocumentResource doc)
    {
        var response = await _client.PostAsJsonAsync("/api/document", doc);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<DocumentResource>())!;
    }

    // -----------------------------------------------------------------------
    // POST /api/document
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Create_ValidDocument_ReturnsCreated()
    {
        var doc = MakeDocument();

        var response = await _client.PostAsJsonAsync("/api/document", doc);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Create_ValidDocument_ReturnsDocumentWithId()
    {
        var doc = MakeDocument();

        var created = await CreateDocumentViaApi(doc);

        Assert.NotNull(created);
        Assert.False(string.IsNullOrEmpty(created.Id));
        Assert.Equal("Test NPC", created.EntityId);
    }

    [Fact]
    public async Task Create_ValidDocument_LocationHeaderPointsToGetById()
    {
        var doc = MakeDocument();

        var response = await _client.PostAsJsonAsync("/api/document", doc);

        Assert.NotNull(response.Headers.Location);
        Assert.Contains("/api/document/", response.Headers.Location.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    // -----------------------------------------------------------------------
    // GET /api/document?campaignId=x
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetByCampaign_WithDocuments_ReturnsMatchingDocuments()
    {
        var campaignId = $"campaign-{Guid.NewGuid():N}";
        await CreateDocumentViaApi(MakeDocument(campaignId, "NPC 1"));
        await CreateDocumentViaApi(MakeDocument(campaignId, "NPC 2"));

        var docs = await _client.GetFromJsonAsync<List<DocumentResource>>($"/api/document?campaignId={campaignId}");

        Assert.NotNull(docs);
        Assert.Equal(2, docs.Count);
    }

    [Fact]
    public async Task GetByCampaign_NoCampaignDocuments_ReturnsEmptyList()
    {
        var docs = await _client.GetFromJsonAsync<List<DocumentResource>>("/api/document?campaignId=no-docs-here");

        Assert.NotNull(docs);
        Assert.Empty(docs);
    }

    // -----------------------------------------------------------------------
    // GET /api/document/{id}
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetById_ExistingDocument_ReturnsDocument()
    {
        var created = await CreateDocumentViaApi(MakeDocument());

        var doc = await _client.GetFromJsonAsync<DocumentResource>($"/api/document/{created.Id}");

        Assert.NotNull(doc);
        Assert.Equal(created.Id, doc.Id);
    }

    [Fact]
    public async Task GetById_UnknownId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/document/does-not-exist");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // -----------------------------------------------------------------------
    // PUT /api/document/{id}
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Update_ExistingDocument_ReturnsOkWithUpdatedData()
    {
        var created = await CreateDocumentViaApi(MakeDocument());
        var updated = new DocumentResource
        {
            Id = created.Id,
            EntityId = "Updated NPC Name",
            CampaignId = created.CampaignId,
            Data = created.Data
        };

        var response = await _client.PutAsJsonAsync($"/api/document/{created.Id}", updated);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<DocumentResource>();
        Assert.Equal("Updated NPC Name", result!.EntityId);
    }

    [Fact]
    public async Task Update_IdMismatch_ReturnsBadRequest()
    {
        var created = await CreateDocumentViaApi(MakeDocument());
        var updated = new DocumentResource { Id = "different-id", EntityId = created.EntityId, CampaignId = created.CampaignId, Data = created.Data };

        var response = await _client.PutAsJsonAsync($"/api/document/{created.Id}", updated);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // -----------------------------------------------------------------------
    // DELETE /api/document/{id}
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Delete_ExistingDocument_ReturnsNoContent()
    {
        var created = await CreateDocumentViaApi(MakeDocument());

        var response = await _client.DeleteAsync($"/api/document/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ThenGetById_ReturnsNotFound()
    {
        var created = await CreateDocumentViaApi(MakeDocument());
        await _client.DeleteAsync($"/api/document/{created.Id}");

        var response = await _client.GetAsync($"/api/document/{created.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
