using API.Services.Documents;
using MediatR;
using Mediator.Mediator.Contracts.Documents;
using Mediator.Mediator.Handlers.Documents;
using Models.Common;

namespace DemonsAndDogs.API.Tests.Handlers;

// ---------------------------------------------------------------------------
// Fakes
// ---------------------------------------------------------------------------

file class FakeDocumentService : IDocumentService
{
    public List<DocumentResource> Documents { get; set; } = [];
    public DocumentResource? LastCreated { get; private set; }
    public DocumentResource? LastUpdated { get; private set; }
    public string? LastDeletedId { get; private set; }

    public Task<IEnumerable<DocumentResource>> GetByCampaignAsync(string campaignId, CancellationToken cancellationToken = default) =>
        Task.FromResult<IEnumerable<DocumentResource>>(Documents.Where(d => d.CampaignId == campaignId).ToList());

    public Task<DocumentResource?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        Task.FromResult(Documents.FirstOrDefault(d => d.Id == id));

    public Task<DocumentResource> CreateAsync(DocumentResource resource, CancellationToken cancellationToken = default)
    {
        LastCreated = resource;
        Documents.Add(resource);
        return Task.FromResult(resource);
    }

    public Task<DocumentResource> UpdateAsync(DocumentResource resource, CancellationToken cancellationToken = default)
    {
        LastUpdated = resource;
        return Task.FromResult(resource);
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        LastDeletedId = id;
        Documents.RemoveAll(d => d.Id == id);
        return Task.CompletedTask;
    }
}

// ---------------------------------------------------------------------------
// Tests
// ---------------------------------------------------------------------------

/// <summary>
/// Unit tests for document MediatR handlers.
/// Documents support full CRUD: list by campaign, get, create, update, delete.
/// All handlers delegate directly to IDocumentService.
/// </summary>
public class DocumentHandlersTests
{
    // -----------------------------------------------------------------------
    // GetDocumentsByCampaignHandler
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetDocumentsByCampaign_WithMatchingDocs_ReturnsFiltered()
    {
        var service = new FakeDocumentService
        {
            Documents =
            [
                new() { Id = "d1", CampaignId = "c1", EntityId = "Innkeeper" },
                new() { Id = "d2", CampaignId = "c1", EntityId = "Tavern" },
                new() { Id = "d3", CampaignId = "c2", EntityId = "Goblin King" }
            ]
        };
        var handler = new GetDocumentsByCampaignHandler(service);

        var result = await handler.Handle(new GetDocumentsByCampaignRequest("c1"), default);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetDocumentsByCampaign_NoMatches_ReturnsEmpty()
    {
        var handler = new GetDocumentsByCampaignHandler(new FakeDocumentService());

        var result = await handler.Handle(new GetDocumentsByCampaignRequest("c1"), default);

        Assert.Empty(result);
    }

    // -----------------------------------------------------------------------
    // GetDocumentHandler
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetDocument_ExistingId_ReturnsDocument()
    {
        var service = new FakeDocumentService
        {
            Documents = [new() { Id = "d1", EntityId = "Innkeeper" }]
        };
        var handler = new GetDocumentHandler(service);

        var result = await handler.Handle(new GetDocumentRequest("d1"), default);

        Assert.NotNull(result);
        Assert.Equal("Innkeeper", result.EntityId);
    }

    [Fact]
    public async Task GetDocument_UnknownId_ReturnsNull()
    {
        var handler = new GetDocumentHandler(new FakeDocumentService());

        var result = await handler.Handle(new GetDocumentRequest("unknown"), default);

        Assert.Null(result);
    }

    // -----------------------------------------------------------------------
    // CreateDocumentHandler
    // -----------------------------------------------------------------------

    [Fact]
    public async Task CreateDocument_ValidResource_DelegatesToServiceAndReturns()
    {
        var service = new FakeDocumentService();
        var handler = new CreateDocumentHandler(service);
        var doc = new DocumentResource { Id = "d1", EntityId = "New NPC", CampaignId = "c1" };

        var result = await handler.Handle(new CreateDocumentRequest(doc), default);

        Assert.Equal("New NPC", result.EntityId);
        Assert.Equal(doc, service.LastCreated);
    }

    // -----------------------------------------------------------------------
    // UpdateDocumentHandler
    // -----------------------------------------------------------------------

    [Fact]
    public async Task UpdateDocument_ValidResource_DelegatesToServiceAndReturns()
    {
        var service = new FakeDocumentService();
        var handler = new UpdateDocumentHandler(service);
        var doc = new DocumentResource { Id = "d1", EntityId = "Updated NPC" };

        var result = await handler.Handle(new UpdateDocumentRequest(doc), default);

        Assert.Equal("Updated NPC", result.EntityId);
        Assert.Equal(doc, service.LastUpdated);
    }

    // -----------------------------------------------------------------------
    // DeleteDocumentHandler
    // -----------------------------------------------------------------------

    [Fact]
    public async Task DeleteDocument_ValidId_DelegatesToServiceAndReturnsUnit()
    {
        var service = new FakeDocumentService
        {
            Documents = [new() { Id = "d1" }]
        };
        var handler = new DeleteDocumentHandler(service);

        var result = await handler.Handle(new DeleteDocumentRequest("d1"), default);

        Assert.Equal(Unit.Value, result);
        Assert.Equal("d1", service.LastDeletedId);
    }
}
