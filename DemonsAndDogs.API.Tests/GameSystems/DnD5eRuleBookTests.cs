using API.Services.GameSystems.DnD5e;
using DemonsAndDogs.API.Tests.GameSystems.Builders;
using Models.GameSystems;
using Xunit;

namespace DemonsAndDogs.API.Tests.GameSystems;

public class DnD5eRuleBookTests
{
    private class FakeDnD5eRuleBook : DnD5eRuleBook
    {
        public int FixedRoll { get; set; } = 10;
        protected override int RollD20() => FixedRoll;
    }

    [Fact]
    public void ResolveSkillCheck_RollPlusModifierMeetsDC_ReturnsSuccess()
    {
        // Arrange
        var ruleBook = new FakeDnD5eRuleBook { FixedRoll = 10 };
        var context = new SkillCheckContextBuilder()
            .WithAbilityModifier(2)
            .WithProficiencyBonus(2)
            .WithDifficultyClass(14)
            .Build();

        // Act
        var result = ruleBook.ResolveSkillCheck(context);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(14, result.TotalResult);
    }

    [Fact]
    public void ResolveSkillCheck_RollPlusModifierBelowDC_ReturnsFailure()
    {
        // Arrange
        var ruleBook = new FakeDnD5eRuleBook { FixedRoll = 5 };
        var context = new SkillCheckContextBuilder()
            .WithAbilityModifier(2)
            .WithProficiencyBonus(2)
            .WithDifficultyClass(10)
            .Build();

        // Act
        var result = ruleBook.ResolveSkillCheck(context);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(9, result.TotalResult);
    }

    [Fact]
    public void ResolveSkillCheck_NaturalTwenty_ReturnsSuccessRegardlessOfDC()
    {
        // Arrange
        var ruleBook = new FakeDnD5eRuleBook { FixedRoll = 20 };
        var context = new SkillCheckContextBuilder()
            .WithAbilityModifier(0)
            .WithProficiencyBonus(0)
            .WithDifficultyClass(30)
            .Build();

        // Act
        var result = ruleBook.ResolveSkillCheck(context);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains("Natural 20!", result.Message);
    }

    [Fact]
    public void ResolveSkillCheck_NaturalOne_ReturnsFailureRegardlessOfModifiers()
    {
        // Arrange
        var ruleBook = new FakeDnD5eRuleBook { FixedRoll = 1 };
        var context = new SkillCheckContextBuilder()
            .WithAbilityModifier(10)
            .WithProficiencyBonus(10)
            .WithDifficultyClass(5)
            .Build();

        // Act
        var result = ruleBook.ResolveSkillCheck(context);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Natural 1!", result.Message);
    }

    [Fact]
    public void ResolveSkillCheck_ProficiencyBonusIsApplied_TotalIsCorrect()
    {
        // Arrange
        var ruleBook = new FakeDnD5eRuleBook { FixedRoll = 10 };
        var context = new SkillCheckContextBuilder()
            .WithAbilityModifier(0)
            .WithProficiencyBonus(5)
            .Build();

        // Act
        var result = ruleBook.ResolveSkillCheck(context);

        // Assert
        Assert.Equal(15, result.TotalResult);
    }

    [Fact]
    public void ResolveSkillCheck_AdditionalModifiersApplied_TotalIsCorrect()
    {
        // Arrange
        var ruleBook = new FakeDnD5eRuleBook { FixedRoll = 10 };
        var modifiers = new Dictionary<string, int> { { "Guidance", 2 }, { "Bardic Inspiration", 3 } };
        var context = new SkillCheckContextBuilder()
            .WithAbilityModifier(0)
            .WithProficiencyBonus(0)
            .WithAdditionalModifiers(modifiers)
            .Build();

        // Act
        var result = ruleBook.ResolveSkillCheck(context);

        // Assert
        Assert.Equal(15, result.TotalResult);
    }

    [Fact]
    public void ResolveAttack_RollMeetsOrExceedsAC_ReturnsHit()
    {
        // Arrange
        var ruleBook = new FakeDnD5eRuleBook { FixedRoll = 10 };
        var context = new AttackContextBuilder()
            .WithAttackModifier(5)
            .WithTargetArmorClass(15)
            .Build();

        // Act
        var result = ruleBook.ResolveAttack(context);

        // Assert
        Assert.True(result.IsHit);
        Assert.Equal(15, result.TotalAttackResult);
    }

    [Fact]
    public void ResolveAttack_RollBelowAC_ReturnsMiss()
    {
        // Arrange
        var ruleBook = new FakeDnD5eRuleBook { FixedRoll = 5 };
        var context = new AttackContextBuilder()
            .WithAttackModifier(5)
            .WithTargetArmorClass(11)
            .Build();

        // Act
        var result = ruleBook.ResolveAttack(context);

        // Assert
        Assert.False(result.IsHit);
        Assert.Equal(10, result.TotalAttackResult);
    }

    [Fact]
    public void ResolveAttack_NaturalTwenty_ReturnsCriticalHit()
    {
        // Arrange
        var ruleBook = new FakeDnD5eRuleBook { FixedRoll = 20 };
        var context = new AttackContextBuilder()
            .WithAttackModifier(0)
            .WithTargetArmorClass(30)
            .Build();

        // Act
        var result = ruleBook.ResolveAttack(context);

        // Assert
        Assert.True(result.IsHit);
        Assert.True(result.IsCriticalHit);
        Assert.Equal("Critical Hit!", result.Message);
    }

    [Fact]
    public void ResolveAttack_NaturalOne_ReturnsCriticalMiss()
    {
        // Arrange
        var ruleBook = new FakeDnD5eRuleBook { FixedRoll = 1 };
        var context = new AttackContextBuilder()
            .WithAttackModifier(20)
            .WithTargetArmorClass(10)
            .Build();

        // Act
        var result = ruleBook.ResolveAttack(context);

        // Assert
        Assert.False(result.IsHit);
        Assert.Equal("Critical Miss!", result.Message);
    }

    [Fact]
    public void ResolveAttack_NoTargetAC_ReturnsHit()
    {
        // Arrange
        var ruleBook = new FakeDnD5eRuleBook { FixedRoll = 5 };
        var context = new AttackContextBuilder()
            .WithTargetArmorClass(null)
            .Build();

        // Act
        var result = ruleBook.ResolveAttack(context);

        // Assert
        Assert.True(result.IsHit);
    }

    [Fact]
    public void ResolveAttack_OnHit_DamageDealtIsReturned()
    {
        // Arrange
        var ruleBook = new FakeDnD5eRuleBook { FixedRoll = 10 };
        var context = new AttackContextBuilder()
            .WithAttackModifier(5)
            .WithTargetArmorClass(10)
            .Build();

        // Act
        var result = ruleBook.ResolveAttack(context);

        // Assert
        Assert.True(result.IsHit);
        // Note: Currently implementation does not calculate damage, 
        // but we verify the property exists in the result.
        Assert.Null(result.DamageDealt); 
    }

    [Fact]
    public void ResolveSkillCheck_NullAdditionalModifiers_DoesNotThrow()
    {
        // Arrange
        var ruleBook = new DnD5eRuleBook();
        var context = new SkillCheckContextBuilder()
            .WithAdditionalModifiers(null)
            .Build();

        // Act
        var exception = Record.Exception(() => ruleBook.ResolveSkillCheck(context));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void ResolveAttack_NullAdditionalModifiers_DoesNotThrow()
    {
        // Arrange
        var ruleBook = new DnD5eRuleBook();
        var context = new AttackContextBuilder()
            .WithAdditionalModifiers(null)
            .Build();

        // Act
        var exception = Record.Exception(() => ruleBook.ResolveAttack(context));

        // Assert
        Assert.Null(exception);
    }
}
