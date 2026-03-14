using API.Services.Document;
using DataAccess.Abstraction;
using Models.Common;
using Xunit;

namespace DemonsAndDogs.API.Tests.Documents;

// ---------------------------------------------------------------------------
// Fakes
// ---------------------------------------------------------------------------

file class FakeRepository : IJsonResourceRepository
{
    private readonly List<JsonResource> _store = new();

    public void Seed(params JsonResource[] resources) => _store.AddRange(resources);

    public Task<JsonResource?> GetByIdAsync(string id) =>
        Task.FromResult(_store.FirstOrDefault(r => r.Id == id));

    public Task<IEnumerable<JsonResource>> GetAllAsync() =>
        Task.FromResult<IEnumerable<JsonResource>>(_store.ToList());

    public Task<IEnumerable<JsonResource>> GetByResourceKindAsync(string resourceKind) =>
        Task.FromResult<IEnumerable<JsonResource>>(_store.Where(r => r.ResourceKind == resourceKind).ToList());

    public Task<IEnumerable<JsonResource>> QueryAsync(Func<IQueryable<JsonResource>, IQueryable<JsonResource>> query) =>
        Task.FromResult<IEnumerable<JsonResource>>(query(_store.AsQueryable()).ToList());

    public Task<JsonResource> CreateAsync(JsonResource resource)
    {
        _store.Add(resource);
        return Task.FromResult(resource);
    }

    public Task<JsonResource> UpdateAsync(JsonResource resource)
    {
        var idx = _store.FindIndex(r => r.Id == resource.Id);
        if (idx < 0) throw new KeyNotFoundException(resource.Id);
        _store[idx] = resource;
        return Task.FromResult(resource);
    }

    public Task DeleteAsync(string id)
    {
        _store.RemoveAll(r => r.Id == id);
        return Task.CompletedTask;
    }
}

// ---------------------------------------------------------------------------
// JsonDocumentService Tests
// ---------------------------------------------------------------------------

public class JsonDocumentServiceTests
{
    // -----------------------------------------------------------------------
    // GetByCampaignAsync
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetByCampaignAsync_DocumentsExistForCampaign_ReturnsOnlyMatching()
    {
        // Arrange
        var repo = new FakeRepository();
        repo.Seed(
            new DocumentResource { Id = "d1", CampaignId = "c1" },
            new DocumentResource { Id = "d2", CampaignId = "c1" });
        var sut = new JsonDocumentService(repo);

        // Act
        var result = await sut.GetByCampaignAsync("c1");

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByCampaignAsync_NoDocumentsForCampaign_ReturnsEmpty()
    {
        // Arrange
        var repo = new FakeRepository();
        var sut = new JsonDocumentService(repo);

        // Act
        var result = await sut.GetByCampaignAsync("c1");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByCampaignAsync_MixedCampaignIds_ReturnsOnlyCorrectCampaign()
    {
        // Arrange
        var repo = new FakeRepository();
        repo.Seed(
            new DocumentResource { Id = "d1", CampaignId = "c1" },
            new DocumentResource { Id = "d2", CampaignId = "c2" },
            new DocumentResource { Id = "d3", CampaignId = "c1" },
            new CampaignResource { Id = "c1" });
        var sut = new JsonDocumentService(repo);

        // Act
        var result = await sut.GetByCampaignAsync("c1");

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, d => Assert.Equal("c1", d.CampaignId));
    }

    // -----------------------------------------------------------------------
    // GetByIdAsync
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetByIdAsync_ExistingDocument_ReturnsDocument()
    {
        // Arrange
        var repo = new FakeRepository();
        repo.Seed(new DocumentResource { Id = "d1", CampaignId = "c1" });
        var sut = new JsonDocumentService(repo);

        // Act
        var result = await sut.GetByIdAsync("d1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("d1", result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_UnknownId_ReturnsNull()
    {
        // Arrange
        var repo = new FakeRepository();
        var sut = new JsonDocumentService(repo);

        // Act
        var result = await sut.GetByIdAsync("does-not-exist");

        // Assert
        Assert.Null(result);
    }

    // -----------------------------------------------------------------------
    // CreateAsync
    // -----------------------------------------------------------------------

    [Fact]
    public async Task CreateAsync_ValidDocument_StoresAndReturnsDocument()
    {
        // Arrange
        var repo = new FakeRepository();
        var sut = new JsonDocumentService(repo);
        var doc = new DocumentResource { Id = "d1", CampaignId = "c1", EntityId = "Gareth the Smith" };

        // Act
        var result = await sut.CreateAsync(doc);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("d1", result.Id);
        Assert.Equal("Gareth the Smith", result.EntityId);
        Assert.Single(await repo.GetAllAsync());
    }

    // -----------------------------------------------------------------------
    // UpdateAsync
    // -----------------------------------------------------------------------

    [Fact]
    public async Task UpdateAsync_ExistingDocument_UpdatesAndReturns()
    {
        // Arrange
        var repo = new FakeRepository();
        repo.Seed(new DocumentResource { Id = "d1", CampaignId = "c1", EntityId = "Old Title" });
        var sut = new JsonDocumentService(repo);
        var updated = new DocumentResource { Id = "d1", CampaignId = "c1", EntityId = "New Title" };

        // Act
        var result = await sut.UpdateAsync(updated);

        // Assert
        Assert.Equal("New Title", result.EntityId);
        var stored = await repo.GetByIdAsync("d1");
        Assert.Equal("New Title", (stored as DocumentResource)!.EntityId);
    }

    // -----------------------------------------------------------------------
    // DeleteAsync
    // -----------------------------------------------------------------------

    [Fact]
    public async Task DeleteAsync_ExistingDocument_RemovesFromRepository()
    {
        // Arrange
        var repo = new FakeRepository();
        repo.Seed(new DocumentResource { Id = "d1", CampaignId = "c1" });
        var sut = new JsonDocumentService(repo);

        // Act
        await sut.DeleteAsync("d1");

        // Assert
        Assert.Empty(await repo.GetAllAsync());
    }
}
