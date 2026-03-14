using Xunit;

namespace DemonsAndDogs.API.Tests.SessionPersistence;

/// <summary>
/// TDD stubs for session-persistence feature.
/// See docs/features/session-persistence.md for full spec.
/// </summary>
public class SessionPersistenceTests
{
    // Happy Path

    [Fact]
    public async Task SaveAsync_ValidSessionState_PersistsSessionResourceWithCorrectKind()
    {
        // Arrange
        // Act
        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task SaveAsync_ValidSessionState_DataContainsSerializedEventLog()
    {
        // Arrange
        // Act
        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task SaveAsync_ExistingSession_UpdatesExistingResourceRatherThanCreatingDuplicate()
    {
        // Arrange
        // Act
        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task LoadAsync_ExistingSessionId_ReturnsRehydratedSessionStateWithEventLog()
    {
        // Arrange
        // Act
        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task LoadAsync_ExistingSessionId_SessionStateMatchesOriginal()
    {
        // Arrange
        // Act
        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task StartSessionHandler_NewSession_SessionIsSavedToRepository()
    {
        // Arrange
        // Act
        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task PerformActionHandler_AfterAction_UpdatedSessionIsSavedToRepository()
    {
        // Arrange
        // Act
        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task GetSessionHandler_SessionNotInMemory_LoadsFromRepositoryAndCachesInStore()
    {
        // Arrange
        // Act
        // Assert
        throw new NotImplementedException();
    }

    // Edge Cases

    [Fact]
    public async Task SaveAsync_SessionWithEmptyEventLog_PersistsSuccessfully()
    {
        // Arrange
        // Act
        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task LoadAsync_SessionWithEmptyEventLog_ReturnsSessionStateWithEmptyList()
    {
        // Arrange
        // Act
        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task GetSessionHandler_SessionAlreadyInMemory_DoesNotCallRepository()
    {
        // Arrange
        // Act
        // Assert
        throw new NotImplementedException();
    }

    // Error Cases

    [Fact]
    public async Task LoadAsync_UnknownSessionId_ReturnsNull()
    {
        // Arrange
        // Act
        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public async Task GetSessionHandler_SessionNotInMemoryOrRepository_ThrowsOrReturnsNotFound()
    {
        // Arrange
        // Act
        // Assert
        throw new NotImplementedException();
    }
}
