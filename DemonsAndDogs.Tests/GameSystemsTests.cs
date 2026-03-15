using API.Services.GameSystems;
using API.Services.GameSystems.DnD5e;
using Models.GameSystems;
using Xunit;

namespace DemonsAndDogs.Tests;

public class GameSystemsTests
{
    [Fact]
    public void DnD5eRuleBook_SkillCheck_ReturnsValidResult()
    {
        var ruleBook = new DnD5eRuleBook();
        var context = new SkillCheckContext("test-char", "stealth", 3, 2, 15);
        
        var result = ruleBook.ResolveSkillCheck(context);
        
        Assert.InRange(result.RollValue, 1, 20);
        Assert.Equal(result.RollValue + 3 + 2, result.TotalResult);
        Assert.Equal(result.TotalResult >= 15, result.IsSuccess);
    }

    [Fact]
    public void DnD5eRuleBook_Attack_ReturnsValidResult()
    {
        var ruleBook = new DnD5eRuleBook();
        var context = new AttackContext("longsword", 5, 14);
        
        var result = ruleBook.ResolveAttack(context);
        
        Assert.InRange(result.AttackRoll, 1, 20);
        Assert.Equal(result.AttackRoll + 5, result.TotalAttackResult);
        if (result.AttackRoll == 20) Assert.True(result.IsCriticalHit);
        if (result.IsCriticalHit) Assert.True(result.IsHit);
        else Assert.Equal(result.TotalAttackResult >= 14, result.IsHit);
    }

    [Fact]
    public void GameSystemRegistry_DiscoversDnD5e()
    {
        var registry = new GameSystemRegistry();
        var ruleBook = registry.Get("dnd5e");
        
        Assert.NotNull(ruleBook);
        Assert.IsType<DnD5eRuleBook>(ruleBook);
        Assert.Equal("dnd5e", ruleBook.SystemId);
    }
}
