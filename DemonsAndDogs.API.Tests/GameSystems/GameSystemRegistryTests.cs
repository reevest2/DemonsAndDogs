using API.Services.GameSystems;
using API.Services.GameSystems.DnD5e;
using Models;
using Xunit;

namespace DemonsAndDogs.API.Tests.GameSystems;

public class GameSystemRegistryTests
{
    private const string DnD5eSystemId = "dnd5e";

    [Fact]
    public void GameSystemRegistry_DiscoversDnD5eRuleBook_ViaReflection()
    {
        var registry = new GameSystemRegistry();
        var systems = registry.GetAll();

        Assert.Contains(systems, s => s is DnD5eRuleBook);
    }

    [Fact]
    public void GameSystemRegistry_UnknownSystemId_ReturnsNotFound()
    {
        var registry = new GameSystemRegistry();

        var result = registry.Get("unknown-system-id");

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.NotFound, result.Error!.Code);
    }

    [Fact]
    public void Get_DnD5e_ReturnsDnD5eRuleBook()
    {
        var registry = new GameSystemRegistry();

        var result = registry.Get(DnD5eSystemId);

        Assert.True(result.IsSuccess);
        Assert.IsType<DnD5eRuleBook>(result.Value);
        Assert.Equal(DnD5eSystemId, result.Value!.SystemId);
    }
}
