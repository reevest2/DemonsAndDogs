using Bunit;
using Models.GameSystems;
using System.Collections.Generic;
using UIComponents;
using Xunit;

namespace DemonsAndDogs.Tests.Components;

public class CharacterSheetComponentTests : TestContext
{
    [Fact]
    public void CharacterSheet_RendersAllSections()
    {
        // Arrange
        var schema = new CharacterSheetSchema("dnd5e", new List<SheetSection>
        {
            new SheetSection("Attributes", "Attributes", new List<SheetField>()),
            new SheetSection("Skills", "Skills", new List<SheetField>())
        });

        // Act
        var cut = Render<CharacterSheetComponent>(parameters => parameters
            .Add(p => p.Schema, schema));

        // Assert
        var headers = cut.FindAll("h4");
        Assert.Equal(2, headers.Count);
        Assert.Equal("Attributes", headers[0].TextContent);
        Assert.Equal("Skills", headers[1].TextContent);
    }

    [Fact]
    public void CharacterSheet_RendersFields()
    {
        // Arrange
        var schema = new CharacterSheetSchema("dnd5e", new List<SheetSection>
        {
            new SheetSection("Attributes", "Attributes", new List<SheetField>
            {
                new SheetField("Strength", "STR", "number", true, 15),
                new SheetField("Dexterity", "DEX", "number", true, 12)
            })
        });

        // Act
        var cut = Render<CharacterSheetComponent>(parameters => parameters
            .Add(p => p.Schema, schema));

        // Assert
        var fieldLabels = cut.FindAll(".small.opacity-75");
        Assert.Equal(2, fieldLabels.Count);
        Assert.Equal("STR", fieldLabels[0].TextContent);
        Assert.Equal("DEX", fieldLabels[1].TextContent);

        var fieldValues = cut.FindAll(".text-center.py-1");
        Assert.Contains("15", fieldValues[0].TextContent);
        Assert.Contains("12", fieldValues[1].TextContent);

        // Verify read-only: no inputs should be present
        var inputs = cut.FindAll("input");
        Assert.Empty(inputs);
    }

    [Fact]
    public void CharacterSheet_NoSchema_RendersEmpty()
    {
        // Act
        var cut = Render<CharacterSheetComponent>(parameters => parameters
            .Add(p => p.Schema, null));

        // Assert
        Assert.Contains("No schema loaded.", cut.Markup);
    }
}
