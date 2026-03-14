using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Models.Common;
using Models.GameSystems;
using System.Collections.Generic;
using System.Text.Json;
using UIComponents;
using Xunit;
using DemonsAndDogs.Tests.Fakes;

namespace DemonsAndDogs.Tests.Components;

public class BuilderDashboardComponentTests : TestContext
{
    private readonly FakeCampaignClient _campaignClient;
    private readonly FakeCharacterClient _characterClient;
    private readonly FakeGameSystemClient _gameSystemClient;

    public BuilderDashboardComponentTests()
    {
        _campaignClient = new FakeCampaignClient();
        _characterClient = new FakeCharacterClient();
        _gameSystemClient = new FakeGameSystemClient();

        Services.AddSingleton<API.Client.Abstraction.ICampaignClient>(_campaignClient);
        Services.AddSingleton<API.Client.Abstraction.ICharacterClient>(_characterClient);
        Services.AddSingleton<API.Client.Abstraction.IGameSystemClient>(_gameSystemClient);
    }

    [Fact]
    public void Dashboard_ShowsTitle()
    {
        var cut = Render<BuilderDashboardComponent>();

        Assert.Contains("Demons", cut.Markup);
        Assert.Contains("Campaign Workshop", cut.Markup);
    }

    [Fact]
    public void Dashboard_ShowsStatCounts()
    {
        var data = JsonDocument.Parse("{}").RootElement;
        _campaignClient.Campaigns = new List<CampaignResource>
        {
            new() { EntityId = "Campaign 1", Data = data },
            new() { EntityId = "Campaign 2", Data = data }
        };
        _characterClient.Characters = new List<CharacterResource>
        {
            new() { EntityId = "Hero 1" }
        };
        _gameSystemClient.Systems = new List<GameSystemDescriptor>
        {
            new("dnd5e", "D&D 5th Edition")
        };

        var cut = Render<BuilderDashboardComponent>();

        var statCards = cut.FindAll(".stat-card");
        Assert.Equal(3, statCards.Count);
        Assert.Contains("2", statCards[0].InnerHtml); // campaigns
        Assert.Contains("1", statCards[1].InnerHtml); // characters
        Assert.Contains("1", statCards[2].InnerHtml); // game systems
    }

    [Fact]
    public void Dashboard_ShowsCampaignCards_WhenDataExists()
    {
        var data = JsonDocument.Parse("{\"description\":\"A dark adventure\"}").RootElement;
        _campaignClient.Campaigns = new List<CampaignResource>
        {
            new() { Id = "c1", EntityId = "Dark Crusade", Data = data }
        };

        var cut = Render<BuilderDashboardComponent>();

        var cards = cut.FindAll(".campaign-card");
        Assert.Single(cards);
        Assert.Contains("Dark Crusade", cut.Markup);
        Assert.Contains("A dark adventure", cut.Markup);
    }

    [Fact]
    public void Dashboard_ShowsEmptyState_WhenNoCampaigns()
    {
        _campaignClient.Campaigns = new List<CampaignResource>();

        var cut = Render<BuilderDashboardComponent>();

        Assert.Contains("No campaigns yet", cut.Markup);
        Assert.DoesNotContain("campaign-card", cut.Markup);
    }

    [Fact]
    public void Dashboard_LimitsToThreeCampaigns()
    {
        var data = JsonDocument.Parse("{}").RootElement;
        _campaignClient.Campaigns = new List<CampaignResource>
        {
            new() { Id = "1", EntityId = "C1", Data = data },
            new() { Id = "2", EntityId = "C2", Data = data },
            new() { Id = "3", EntityId = "C3", Data = data },
            new() { Id = "4", EntityId = "C4", Data = data }
        };

        var cut = Render<BuilderDashboardComponent>();

        var cards = cut.FindAll(".campaign-card");
        Assert.Equal(3, cards.Count);
    }

    [Fact]
    public void Dashboard_StatCardsLinkToListPages()
    {
        var cut = Render<BuilderDashboardComponent>();

        var links = cut.FindAll("a[href='/campaigns'], a[href='/characters'], a[href='/systems']");
        Assert.True(links.Count >= 3);
    }
}
