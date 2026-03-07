using Bunit;
using Models.Session;
using Models.GameSystems;
using System;
using System.Collections.Generic;
using UIComponents;
using Xunit;
using System.Linq;

namespace DemonsAndDogs.Tests.Components;

public class ActionLogComponentTests : TestContext
{
    [Fact]
    public void ActionLog_NoEvents_RendersEmptyState()
    {
        // Arrange
        var events = new List<SessionEvent>();

        // Act
        var cut = Render<ActionLogComponent>(parameters => parameters
            .Add(p => p.Events, events));

        // Assert
        cut.Find("p").MarkupMatches("<p class=\"mt-2\">No actions performed yet.</p>");
    }

    [Fact]
    public void ActionLog_WithEvents_RendersCorrectNumber()
    {
        // Arrange
        var events = new List<SessionEvent>
        {
            new SessionEvent("SkillCheck", "Climbed a wall", DateTime.UtcNow.AddMinutes(-5)),
            new SessionEvent("Attack", "Swung a sword", DateTime.UtcNow)
        };

        // Act
        var cut = Render<ActionLogComponent>(parameters => parameters
            .Add(p => p.Events, events));

        // Assert
        var eventItems = cut.FindAll(".mb-2.p-2");
        Assert.Equal(2, eventItems.Count);
    }

    [Fact]
    public void ActionLog_WithEvents_ShowsDescriptions()
    {
        // Arrange
        var events = new List<SessionEvent>
        {
            new SessionEvent("SkillCheck", "Climbed a wall", DateTime.UtcNow)
        };

        // Act
        var cut = Render<ActionLogComponent>(parameters => parameters
            .Add(p => p.Events, events));

        // Assert
        Assert.Contains("Climbed a wall", cut.Markup);
    }

    [Fact]
    public void ActionLog_WithEvents_OrderedByTimestampDescending()
    {
        // Arrange
        var older = new SessionEvent("SkillCheck", "Older Event", DateTime.UtcNow.AddMinutes(-10));
        var newer = new SessionEvent("Attack", "Newer Event", DateTime.UtcNow);
        var events = new List<SessionEvent> { older, newer };

        // Act
        var cut = Render<ActionLogComponent>(parameters => parameters
            .Add(p => p.Events, events));

        // Assert
        var eventItems = cut.FindAll(".mb-2.p-2");
        Assert.Contains("Newer Event", eventItems[0].InnerHtml);
        Assert.Contains("Older Event", eventItems[1].InnerHtml);
    }
}
