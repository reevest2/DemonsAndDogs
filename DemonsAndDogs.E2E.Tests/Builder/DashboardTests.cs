using DemonsAndDogs.E2E.Tests.Fixtures;
using Microsoft.Playwright;
using Xunit.Abstractions;

namespace DemonsAndDogs.E2E.Tests.Builder;

[Collection("E2E")]
public class DashboardTests : E2ETestBase
{
    public DashboardTests(ServerFixture servers, PlaywrightFixture playwright, ITestOutputHelper output)
        : base(servers, playwright, output) { }

    [Fact]
    public async Task Dashboard_LoadsWithStatCards()
    {
        try
        {
            await Page.GotoAsync(BuilderUrl);

            // Wait for stat cards to appear (3 total: Campaigns, Characters, Game Systems)
            var statCards = Page.Locator(".stat-card");
            await statCards.First.WaitForAsync();

            Assert.Equal(3, await statCards.CountAsync());

            // Verify stat labels
            await Expect(Page.GetByText("Campaigns")).ToBeVisibleAsync();
            await Expect(Page.GetByText("Characters")).ToBeVisibleAsync();
            await Expect(Page.GetByText("Game Systems")).ToBeVisibleAsync();
        }
        catch { MarkFailed(); throw; }
    }

    [Fact]
    public async Task Dashboard_ShowsSeededCampaign()
    {
        try
        {
            await Page.GotoAsync(BuilderUrl);

            await Expect(Page.GetByText("Lost Mine of Phandelver")).ToBeVisibleAsync();
            await Expect(Page.GetByText("A classic D&D 5e starter adventure.")).ToBeVisibleAsync();
        }
        catch { MarkFailed(); throw; }
    }

    [Fact]
    public async Task Dashboard_CampaignStatCard_NavigatesToCampaigns()
    {
        try
        {
            await Page.GotoAsync(BuilderUrl);

            // Click the Campaigns stat card link
            await Page.Locator("a[href='/campaigns'] .stat-card").ClickAsync();

            await Page.WaitForURLAsync("**/campaigns");
            Assert.Contains("/campaigns", Page.Url);
        }
        catch { MarkFailed(); throw; }
    }

    [Fact]
    public async Task Dashboard_ViewDetails_NavigatesToCampaignDetail()
    {
        try
        {
            await Page.GotoAsync(BuilderUrl);

            await Page.GetByRole(AriaRole.Link, new() { Name = "View Details" }).First.ClickAsync();

            await Page.WaitForURLAsync("**/campaigns/seed-campaign-1");
            Assert.Contains("/campaigns/seed-campaign-1", Page.Url);
        }
        catch { MarkFailed(); throw; }
    }

    private static ILocatorAssertions Expect(ILocator locator) => Assertions.Expect(locator);
}
