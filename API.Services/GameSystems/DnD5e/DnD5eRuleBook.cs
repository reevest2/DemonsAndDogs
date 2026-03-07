using Models.Attributes;
using Models.GameSystems;
using Models.Interfaces;

namespace API.Services.GameSystems.DnD5e;

[GameSystem("dnd5e")]
public class DnD5eRuleBook : IRuleBook
{
    public string SystemId => "dnd5e";
    public string DisplayName => "Dungeons & Dragons 5th Edition";

    public CheckResult ResolveSkillCheck(SkillCheckContext context)
    {
        var roll = Random.Shared.Next(1, 21);
        var total = roll + context.AbilityModifier + context.ProficiencyBonus;

        if (context.AdditionalModifiers != null)
        {
            foreach (var mod in context.AdditionalModifiers.Values)
            {
                total += mod;
            }
        }

        var isSuccess = context.DifficultyClass == null || total >= context.DifficultyClass;
        var message = isSuccess ? "Success" : "Failure";
        
        if (roll == 20) message = "Natural 20! " + message;
        else if (roll == 1) message = "Natural 1! " + message;

        return new CheckResult(roll, total, isSuccess, message);
    }

    public AttackResult ResolveAttack(AttackContext context)
    {
        var roll = Random.Shared.Next(1, 21);
        var total = roll + context.AttackModifier;

        if (context.AdditionalModifiers != null)
        {
            foreach (var mod in context.AdditionalModifiers.Values)
            {
                total += mod;
            }
        }

        var isCriticalHit = roll == 20;
        var isHit = isCriticalHit || (context.TargetArmorClass == null || total >= context.TargetArmorClass);
        var message = isHit ? "Hit" : "Miss";

        if (isCriticalHit) message = "Critical Hit!";
        else if (roll == 1) message = "Critical Miss!";

        return new AttackResult(
            AttackRoll: roll,
            TotalAttackResult: total,
            IsHit: isHit,
            IsCriticalHit: isCriticalHit,
            Message: message);
    }

    public CharacterSheetSchema GetCharacterSheetSchema()
    {
        return new CharacterSheetSchema(
            SystemId,
            [
                new SheetSection("Abilities", "Ability Scores", [
                    new SheetField("strength", "Strength", "number", true, 10),
                    new SheetField("dexterity", "Dexterity", "number", true, 10),
                    new SheetField("constitution", "Constitution", "number", true, 10),
                    new SheetField("intelligence", "Intelligence", "number", true, 10),
                    new SheetField("wisdom", "Wisdom", "number", true, 10),
                    new SheetField("charisma", "Charisma", "number", true, 10)
                ]),
                new SheetSection("Combat", "Combat Stats", [
                    new SheetField("hp", "Hit Points", "number", true, 10),
                    new SheetField("ac", "Armor Class", "number", true, 10)
                ])
            ]
        );
    }
}
