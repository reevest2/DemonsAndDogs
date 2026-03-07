using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Models.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using UIComponents;
using Xunit;
using DemonsAndDogs.Tests.Fakes;
using System.Threading.Tasks;

namespace DemonsAndDogs.Tests.Components;

public class CampaignsComponentTests : TestContext
{
    private readonly FakeCampaignClient _fakeClient;

    public CampaignsComponentTests()
    {
        _fakeClient = new FakeCampaignClient();
        Services.AddSingleton<API.Client.Abstraction.ICampaignClient>(_fakeClient);
    }

    [Fact]
    public void Campaigns_InitialState_ShowsLoading()
    {
        // Arrange - Client returns a task that doesn't complete immediately
        var tcs = new TaskCompletionSource<IEnumerable<CampaignResource>>();
        _fakeClient.CustomTask = tcs.Task;
        
        // Act
        var cut = Render<CampaignsComponent>();

        // Assert
        Assert.Contains("Loading campaigns...", cut.Markup);
    }

    [Fact]
    public void Campaigns_WithData_RendersCards()
    {
        // Arrange
        var data = JsonDocument.Parse("{\"description\":\"The Frozen North description\"}").RootElement;
        _fakeClient.Campaigns = new List<CampaignResource>
        {
            new CampaignResource { EntityId = "The Frozen North", Data = data }
        };

        // Act
        var cut = Render<CampaignsComponent>();
        // Assert
        var cards = cut.FindAll(".campaign-card");
        Assert.Single(cards);
        Assert.Contains("The Frozen North", cut.Markup);
        Assert.Contains("The Frozen North description", cut.Markup);
    }

    [Fact]
    public void Campaigns_NoData_RendersEmptyMessage()
    {
        // Arrange
        _fakeClient.Campaigns = new List<CampaignResource>();

        // Act
        var cut = Render<CampaignsComponent>();
        // Assert
        Assert.Contains("No campaigns found", cut.Markup);
    }
}
