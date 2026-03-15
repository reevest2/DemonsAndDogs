using API.Services.Campaigns;
using API.Services.Characters;
using DataAccess.Abstraction;
using Models;
using Models.Common;
using Xunit;

namespace DemonsAndDogs.API.Tests.CampaignCharacterServices;

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
// JsonCampaignService Tests
// ---------------------------------------------------------------------------

public class JsonCampaignServiceTests
{
    [Fact]
    public async Task GetAllAsync_TwoCampaignsInRepository_ReturnsBothCampaigns()
    {
        var repo = new FakeRepository();
        repo.Seed(new CampaignResource { Id = "c1" }, new CampaignResource { Id = "c2" });
        var sut = new JsonCampaignService(repo);
        var result = await sut.GetAllAsync();
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ExistingCampaignId_ReturnsCampaign()
    {
        var repo = new FakeRepository();
        repo.Seed(new CampaignResource { Id = "c1" });
        var sut = new JsonCampaignService(repo);
        var result = await sut.GetByIdAsync("c1");
        Assert.NotNull(result);
        Assert.Equal("c1", result.Id);
    }

    [Fact]
    public async Task GetAllAsync_NoCampaignsInRepository_ReturnsEmptyList()
    {
        var repo = new FakeRepository();
        var sut = new JsonCampaignService(repo);
        var result = await sut.GetAllAsync();
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_RepositoryContainsMixedKinds_ReturnsOnlyCampaigns()
    {
        var repo = new FakeRepository();
        repo.Seed(new CampaignResource { Id = "c1" }, new CharacterResource { Id = "ch1" }, new SessionResource { Id = "s1" });
        var sut = new JsonCampaignService(repo);
        var result = await sut.GetAllAsync();
        Assert.Single(result);
        Assert.Equal("c1", result.First().Id);
    }

    [Fact]
    public async Task GetByIdAsync_UnknownCampaignId_ReturnsNull()
    {
        var repo = new FakeRepository();
        var sut = new JsonCampaignService(repo);
        var result = await sut.GetByIdAsync("does-not-exist");
        Assert.Null(result);
    }
}

// ---------------------------------------------------------------------------
// JsonCharacterService Tests
// ---------------------------------------------------------------------------

public class JsonCharacterServiceTests
{
    [Fact]
    public async Task GetAllAsync_TwoCharactersInRepository_ReturnsBothCharacters()
    {
        var repo = new FakeRepository();
        repo.Seed(new CharacterResource { Id = "ch1" }, new CharacterResource { Id = "ch2" });
        var sut = new JsonCharacterService(repo);
        var result = await sut.GetAllAsync();
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ExistingCharacterId_ReturnsCharacter()
    {
        var repo = new FakeRepository();
        repo.Seed(new CharacterResource { Id = "ch1" });
        var sut = new JsonCharacterService(repo);
        var result = await sut.GetByIdAsync("ch1");
        Assert.NotNull(result);
        Assert.Equal("ch1", result.Id);
    }

    [Fact]
    public async Task GetBySystemIdAsync_MatchingGameId_ReturnsOnlyCharactersForThatSystem()
    {
        var repo = new FakeRepository();
        repo.Seed(
            new CharacterResource { Id = "ch1", GameId = "dnd5e" },
            new CharacterResource { Id = "ch2", GameId = "dnd5e" },
            new CharacterResource { Id = "ch3", GameId = "other" });
        var sut = new JsonCharacterService(repo);
        var result = await sut.GetBySystemIdAsync("dnd5e");
        Assert.Equal(2, result.Count());
        Assert.All(result, c => Assert.Equal("dnd5e", c.GameId));
    }

    [Fact]
    public async Task GetAllAsync_NoCharactersInRepository_ReturnsEmptyList()
    {
        var repo = new FakeRepository();
        var sut = new JsonCharacterService(repo);
        var result = await sut.GetAllAsync();
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_RepositoryContainsMixedKinds_ReturnsOnlyCharacters()
    {
        var repo = new FakeRepository();
        repo.Seed(new CampaignResource { Id = "c1" }, new CharacterResource { Id = "ch1" }, new SessionResource { Id = "s1" });
        var sut = new JsonCharacterService(repo);
        var result = await sut.GetAllAsync();
        Assert.Single(result);
        Assert.Equal("ch1", result.First().Id);
    }

    [Fact]
    public async Task GetBySystemIdAsync_NoCharactersMatchGameId_ReturnsEmptyList()
    {
        var repo = new FakeRepository();
        repo.Seed(new CharacterResource { Id = "ch1", GameId = "other" });
        var sut = new JsonCharacterService(repo);
        var result = await sut.GetBySystemIdAsync("dnd5e");
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBySystemIdAsync_MultipleSystemsInRepo_ReturnsOnlyRequestedSystem()
    {
        var repo = new FakeRepository();
        repo.Seed(
            new CharacterResource { Id = "ch1", GameId = "dnd5e" },
            new CharacterResource { Id = "ch2", GameId = "pathfinder" },
            new CharacterResource { Id = "ch3", GameId = "dnd5e" });
        var sut = new JsonCharacterService(repo);
        var result = await sut.GetBySystemIdAsync("pathfinder");
        Assert.Single(result);
        Assert.Equal("ch2", result.First().Id);
    }

    [Fact]
    public async Task GetByIdAsync_UnknownCharacterId_ReturnsNull()
    {
        var repo = new FakeRepository();
        var sut = new JsonCharacterService(repo);
        var result = await sut.GetByIdAsync("does-not-exist");
        Assert.Null(result);
    }
}
