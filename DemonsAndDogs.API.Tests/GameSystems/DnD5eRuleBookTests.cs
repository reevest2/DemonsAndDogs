using API.Services.GameSystems.DnD5e;
using Models.GameSystems;
using Xunit;

namespace DemonsAndDogs.API.Tests.GameSystems;

public class DnD5eRuleBookTests
{
    private const string DnD5eSystemId = "dnd5e";

    private class FakeDnD5eRuleBook : DnD5eRuleBook
    {
        public int FixedRoll { get; set; } = 10;
        protected override int RollD20() => FixedRoll;
    }

    [Fact]
    public void ResolveSkillCheck_TotalMeetsDC_Succeeds()
    {
        // Arrange
        var ruleBook = new FakeDnD5eRuleBook { FixedRoll = 10 };
        var context = new SkillCheckContext("char1", "stealth", 2, 2, 14);

        // Act
        var result = ruleBook.ResolveSkillCheck(context);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(14, result.TotalResult);
    }

    [Fact]
    public void ResolveSkillCheck_TotalBelowDC_Fails()
    {
        // Arrange
        var ruleBook = new FakeDnD5eRuleBook { FixedRoll = 5 };
        var context = new SkillCheckContext("char1", "stealth", 2, 2, 10);

        // Act
        var result = ruleBook.ResolveSkillCheck(context);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(9, result.TotalResult);
    }

    [Fact]
    public void ResolveSkillCheck_Natural20_AlwaysSucceeds()
    {
        // Arrange
        var ruleBook = new FakeDnD5eRuleBook { FixedRoll = 20 };
        var context = new SkillCheckContext("char1", "stealth", 0, 0, 30);

        // Act
        var result = ruleBook.ResolveSkillCheck(context);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains("Natural 20!", result.Message);
    }

    [Fact]
    public void ResolveSkillCheck_Natural1_AlwaysFails()
    {
        // Arrange
        var ruleBook = new FakeDnD5eRuleBook { FixedRoll = 1 };
        var context = new SkillCheckContext("char1", "stealth", 10, 10, 5);

        // Act
        var result = ruleBook.ResolveSkillCheck(context);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Natural 1!", result.Message);
    }

    [Fact]
    public void ResolveSkillCheck_NoDC_AlwaysSucceeds()
    {
        // Arrange
        var ruleBook = new FakeDnD5eRuleBook { FixedRoll = 5 };
        var context = new SkillCheckContext("char1", "stealth", 0, 0, null);

        // Act
        var result = ruleBook.ResolveSkillCheck(context);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void ResolveAttack_TotalMeetsAC_Hits()
    {
        // Arrange
        var ruleBook = new FakeDnD5eRuleBook { FixedRoll = 10 };
        var context = new AttackContext("sword", 5, 15);

        // Act
        var result = ruleBook.ResolveAttack(context);

        // Assert
        Assert.True(result.IsHit);
        Assert.Equal(15, result.TotalAttackResult);
    }

    [Fact]
    public void ResolveAttack_TotalBelowAC_Misses()
    {
        // Arrange
        var ruleBook = new FakeDnD5eRuleBook { FixedRoll = 5 };
        var context = new AttackContext("sword", 5, 11);

        // Act
        var result = ruleBook.ResolveAttack(context);

        // Assert
        Assert.False(result.IsHit);
        Assert.Equal(10, result.TotalAttackResult);
    }

    [Fact]
    public void ResolveAttack_Natural20_IsCriticalHit()
    {
        // Arrange
        var ruleBook = new FakeDnD5eRuleBook { FixedRoll = 20 };
        var context = new AttackContext("sword", 0, 30);

        // Act
        var result = ruleBook.ResolveAttack(context);

        // Assert
        Assert.True(result.IsHit);
        Assert.True(result.IsCriticalHit);
        Assert.Equal("Critical Hit!", result.Message);
    }

    [Fact]
    public void ResolveAttack_Natural1_IsCriticalMiss()
    {
        // Arrange
        var ruleBook = new FakeDnD5eRuleBook { FixedRoll = 1 };
        var context = new AttackContext("sword", 20, 10);

        // Act
        var result = ruleBook.ResolveAttack(context);

        // Assert
        Assert.False(result.IsHit);
        Assert.Equal("Critical Miss!", result.Message);
    }

    [Fact]
    public void GetCharacterSheetSchema_ReturnsCorrectSections()
    {
        // Arrange
        var ruleBook = new DnD5eRuleBook();

        // Act
        var schema = ruleBook.GetCharacterSheetSchema();

        // Assert
        Assert.Equal(DnD5eSystemId, schema.SystemId);
        Assert.Contains(schema.Sections, s => s.Name == "Abilities");
        Assert.Contains(schema.Sections, s => s.Name == "Combat");
        var abilitySection = schema.Sections.First(s => s.Name == "Abilities");
        Assert.Contains(abilitySection.Fields, f => f.Key == "strength");
    }
}
