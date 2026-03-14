using API.Services.Campaign;
using API.Services.Character;
using DataAccess.Abstraction;
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
// JsonCampaignService Tests
// ---------------------------------------------------------------------------

public class JsonCampaignServiceTests
{
    // -----------------------------------------------------------------------
    // Happy Path
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetAllAsync_TwoCampaignsInRepository_ReturnsBothCampaigns()
    {
        // Arrange
        var repo = new FakeRepository();
        repo.Seed(new CampaignResource { Id = "c1" }, new CampaignResource { Id = "c2" });
        var sut = new JsonCampaignService(repo);

        // Act
        var result = await sut.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ExistingCampaignId_ReturnsCampaign()
    {
        // Arrange
        var repo = new FakeRepository();
        repo.Seed(new CampaignResource { Id = "c1" });
        var sut = new JsonCampaignService(repo);

        // Act
        var result = await sut.GetByIdAsync("c1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("c1", result.Id);
    }

    // -----------------------------------------------------------------------
    // Edge Cases
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetAllAsync_NoCampaignsInRepository_ReturnsEmptyList()
    {
        // Arrange
        var repo = new FakeRepository();
        var sut = new JsonCampaignService(repo);

        // Act
        var result = await sut.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_RepositoryContainsMixedKinds_ReturnsOnlyCampaigns()
    {
        // Arrange
        var repo = new FakeRepository();
        repo.Seed(new CampaignResource { Id = "c1" }, new CharacterResource { Id = "ch1" }, new SessionResource { Id = "s1" });
        var sut = new JsonCampaignService(repo);

        // Act
        var result = await sut.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("c1", result.First().Id);
    }

    // -----------------------------------------------------------------------
    // Error Cases
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetByIdAsync_UnknownCampaignId_ReturnsNull()
    {
        // Arrange
        var repo = new FakeRepository();
        var sut = new JsonCampaignService(repo);

        // Act
        var result = await sut.GetByIdAsync("does-not-exist");

        // Assert
        Assert.Null(result);
    }
}

// ---------------------------------------------------------------------------
// JsonCharacterService Tests
// ---------------------------------------------------------------------------

public class JsonCharacterServiceTests
{
    // -----------------------------------------------------------------------
    // Happy Path
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetAllAsync_TwoCharactersInRepository_ReturnsBothCharacters()
    {
        // Arrange
        var repo = new FakeRepository();
        repo.Seed(new CharacterResource { Id = "ch1" }, new CharacterResource { Id = "ch2" });
        var sut = new JsonCharacterService(repo);

        // Act
        var result = await sut.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ExistingCharacterId_ReturnsCharacter()
    {
        // Arrange
        var repo = new FakeRepository();
        repo.Seed(new CharacterResource { Id = "ch1" });
        var sut = new JsonCharacterService(repo);

        // Act
        var result = await sut.GetByIdAsync("ch1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("ch1", result.Id);
    }

    [Fact]
    public async Task GetBySystemIdAsync_MatchingGameId_ReturnsOnlyCharactersForThatSystem()
    {
        // Arrange
        var repo = new FakeRepository();
        repo.Seed(
            new CharacterResource { Id = "ch1", GameId = "dnd5e" },
            new CharacterResource { Id = "ch2", GameId = "dnd5e" },
            new CharacterResource { Id = "ch3", GameId = "other" });
        var sut = new JsonCharacterService(repo);

        // Act
        var result = await sut.GetBySystemIdAsync("dnd5e");

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, c => Assert.Equal("dnd5e", c.GameId));
    }

    // -----------------------------------------------------------------------
    // Edge Cases
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetAllAsync_NoCharactersInRepository_ReturnsEmptyList()
    {
        // Arrange
        var repo = new FakeRepository();
        var sut = new JsonCharacterService(repo);

        // Act
        var result = await sut.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_RepositoryContainsMixedKinds_ReturnsOnlyCharacters()
    {
        // Arrange
        var repo = new FakeRepository();
        repo.Seed(new CampaignResource { Id = "c1" }, new CharacterResource { Id = "ch1" }, new SessionResource { Id = "s1" });
        var sut = new JsonCharacterService(repo);

        // Act
        var result = await sut.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("ch1", result.First().Id);
    }

    [Fact]
    public async Task GetBySystemIdAsync_NoCharactersMatchGameId_ReturnsEmptyList()
    {
        // Arrange
        var repo = new FakeRepository();
        repo.Seed(new CharacterResource { Id = "ch1", GameId = "other" });
        var sut = new JsonCharacterService(repo);

        // Act
        var result = await sut.GetBySystemIdAsync("dnd5e");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBySystemIdAsync_MultipleSystemsInRepo_ReturnsOnlyRequestedSystem()
    {
        // Arrange
        var repo = new FakeRepository();
        repo.Seed(
            new CharacterResource { Id = "ch1", GameId = "dnd5e" },
            new CharacterResource { Id = "ch2", GameId = "pathfinder" },
            new CharacterResource { Id = "ch3", GameId = "dnd5e" });
        var sut = new JsonCharacterService(repo);

        // Act
        var result = await sut.GetBySystemIdAsync("pathfinder");

        // Assert
        Assert.Single(result);
        Assert.Equal("ch2", result.First().Id);
    }

    // -----------------------------------------------------------------------
    // Error Cases
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetByIdAsync_UnknownCharacterId_ReturnsNull()
    {
        // Arrange
        var repo = new FakeRepository();
        var sut = new JsonCharacterService(repo);

        // Act
        var result = await sut.GetByIdAsync("does-not-exist");

        // Assert
        Assert.Null(result);
    }
}
