using API.Services.Documents;
using DataAccess.Abstraction;
using Models;
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

    public Task<Result<JsonResource>> UpdateAsync(JsonResource resource)
    {
        var idx = _store.FindIndex(r => r.Id == resource.Id);
        if (idx < 0) return Task.FromResult(Result<JsonResource>.NotFound("JsonResource", resource.Id));
        _store[idx] = resource;
        return Task.FromResult(Result<JsonResource>.Ok(resource));
    }

    public Task<Result> DeleteAsync(string id)
    {
        _store.RemoveAll(r => r.Id == id);
        return Task.FromResult(Result.Ok());
    }
}

// ---------------------------------------------------------------------------
// JsonDocumentService Tests
// ---------------------------------------------------------------------------

public class JsonDocumentServiceTests
{
    [Fact]
    public async Task GetByCampaignAsync_DocumentsExistForCampaign_ReturnsOnlyMatching()
    {
        var repo = new FakeRepository();
        repo.Seed(
            new DocumentResource { Id = "d1", CampaignId = "c1" },
            new DocumentResource { Id = "d2", CampaignId = "c1" });
        var sut = new JsonDocumentService(repo);

        var result = await sut.GetByCampaignAsync("c1");

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByCampaignAsync_NoDocumentsForCampaign_ReturnsEmpty()
    {
        var repo = new FakeRepository();
        var sut = new JsonDocumentService(repo);

        var result = await sut.GetByCampaignAsync("c1");

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByCampaignAsync_MixedCampaignIds_ReturnsOnlyCorrectCampaign()
    {
        var repo = new FakeRepository();
        repo.Seed(
            new DocumentResource { Id = "d1", CampaignId = "c1" },
            new DocumentResource { Id = "d2", CampaignId = "c2" },
            new DocumentResource { Id = "d3", CampaignId = "c1" },
            new CampaignResource { Id = "c1" });
        var sut = new JsonDocumentService(repo);

        var result = await sut.GetByCampaignAsync("c1");

        Assert.Equal(2, result.Count());
        Assert.All(result, d => Assert.Equal("c1", d.CampaignId));
    }

    [Fact]
    public async Task GetByIdAsync_ExistingDocument_ReturnsDocument()
    {
        var repo = new FakeRepository();
        repo.Seed(new DocumentResource { Id = "d1", CampaignId = "c1" });
        var sut = new JsonDocumentService(repo);

        var result = await sut.GetByIdAsync("d1");

        Assert.NotNull(result);
        Assert.Equal("d1", result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_UnknownId_ReturnsNull()
    {
        var repo = new FakeRepository();
        var sut = new JsonDocumentService(repo);

        var result = await sut.GetByIdAsync("does-not-exist");

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ValidDocument_StoresAndReturnsDocument()
    {
        var repo = new FakeRepository();
        var sut = new JsonDocumentService(repo);
        var doc = new DocumentResource { Id = "d1", CampaignId = "c1", EntityId = "Gareth the Smith" };

        var result = await sut.CreateAsync(doc);

        Assert.NotNull(result);
        Assert.Equal("d1", result.Id);
        Assert.Equal("Gareth the Smith", result.EntityId);
        Assert.Single(await repo.GetAllAsync());
    }

    [Fact]
    public async Task UpdateAsync_ExistingDocument_UpdatesAndReturns()
    {
        var repo = new FakeRepository();
        repo.Seed(new DocumentResource { Id = "d1", CampaignId = "c1", EntityId = "Old Title" });
        var sut = new JsonDocumentService(repo);
        var updated = new DocumentResource { Id = "d1", CampaignId = "c1", EntityId = "New Title" };

        var result = await sut.UpdateAsync(updated);

        Assert.True(result.IsSuccess);
        Assert.Equal("New Title", result.Value!.EntityId);
        var stored = await repo.GetByIdAsync("d1");
        Assert.Equal("New Title", (stored as DocumentResource)!.EntityId);
    }

    [Fact]
    public async Task DeleteAsync_ExistingDocument_RemovesFromRepository()
    {
        var repo = new FakeRepository();
        repo.Seed(new DocumentResource { Id = "d1", CampaignId = "c1" });
        var sut = new JsonDocumentService(repo);

        var result = await sut.DeleteAsync("d1");

        Assert.True(result.IsSuccess);
        Assert.Empty(await repo.GetAllAsync());
    }
}
