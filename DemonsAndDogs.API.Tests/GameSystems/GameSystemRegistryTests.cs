using API.Services.GameSystems;
using API.Services.GameSystems.DnD5e;
using Xunit;

namespace DemonsAndDogs.API.Tests.GameSystems;

public class GameSystemRegistryTests
{
    private const string DnD5eSystemId = "dnd5e";

    [Fact]
    public void GameSystemRegistry_DiscoversDnD5eRuleBook_ViaReflection()
    {
        // Arrange & Act
        var registry = new GameSystemRegistry();
        var systems = registry.GetAll();

        // Assert
        Assert.Contains(systems, s => s is DnD5eRuleBook);
    }

    [Fact]
    public void GameSystemRegistry_UnknownSystemId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var registry = new GameSystemRegistry();

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => registry.Get("unknown-system-id"));
    }

    [Fact]
    public void Get_DnD5e_ReturnsDnD5eRuleBook()
    {
        // Arrange
        var registry = new GameSystemRegistry();

        // Act
        var system = registry.Get(DnD5eSystemId);

        // Assert
        Assert.IsType<DnD5eRuleBook>(system);
        Assert.Equal(DnD5eSystemId, system.SystemId);
    }
}
