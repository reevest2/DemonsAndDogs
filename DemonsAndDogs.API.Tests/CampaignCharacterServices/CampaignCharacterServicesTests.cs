using Xunit;

namespace DemonsAndDogs.API.Tests.CampaignCharacterServices;

// ---------------------------------------------------------------------------
// Fakes
// ---------------------------------------------------------------------------
// (FakeRepository reused from SessionPersistenceTests — will extract to shared
//  test helper if needed during implementation)

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
        // Act
        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task GetByIdAsync_ExistingCampaignId_ReturnsCampaign()
    {
        // Arrange
        // Act
        // Assert
        throw new NotImplementedException();
    }

    // -----------------------------------------------------------------------
    // Edge Cases
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetAllAsync_NoCampaignsInRepository_ReturnsEmptyList()
    {
        // Arrange
        // Act
        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task GetAllAsync_RepositoryContainsMixedKinds_ReturnsOnlyCampaigns()
    {
        // Arrange
        // Act
        // Assert
        throw new NotImplementedException();
    }

    // -----------------------------------------------------------------------
    // Error Cases
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetByIdAsync_UnknownCampaignId_ReturnsNull()
    {
        // Arrange
        // Act
        // Assert
        throw new NotImplementedException();
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
        // Act
        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task GetByIdAsync_ExistingCharacterId_ReturnsCharacter()
    {
        // Arrange
        // Act
        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task GetBySystemIdAsync_MatchingGameId_ReturnsOnlyCharactersForThatSystem()
    {
        // Arrange
        // Act
        // Assert
        throw new NotImplementedException();
    }

    // -----------------------------------------------------------------------
    // Edge Cases
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetAllAsync_NoCharactersInRepository_ReturnsEmptyList()
    {
        // Arrange
        // Act
        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task GetAllAsync_RepositoryContainsMixedKinds_ReturnsOnlyCharacters()
    {
        // Arrange
        // Act
        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task GetBySystemIdAsync_NoCharactersMatchGameId_ReturnsEmptyList()
    {
        // Arrange
        // Act
        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task GetBySystemIdAsync_MultipleSystemsInRepo_ReturnsOnlyRequestedSystem()
    {
        // Arrange
        // Act
        // Assert
        throw new NotImplementedException();
    }

    // -----------------------------------------------------------------------
    // Error Cases
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetByIdAsync_UnknownCharacterId_ReturnsNull()
    {
        // Arrange
        // Act
        // Assert
        throw new NotImplementedException();
    }
}
