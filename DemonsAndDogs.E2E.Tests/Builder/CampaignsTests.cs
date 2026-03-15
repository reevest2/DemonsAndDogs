using DemonsAndDogs.E2E.Tests.Fixtures;
using Microsoft.Playwright;
using Xunit.Abstractions;

namespace DemonsAndDogs.E2E.Tests.Builder;

[Collection("E2E")]
public class CampaignsTests : E2ETestBase
{
    public CampaignsTests(ServerFixture servers, PlaywrightFixture playwright, ITestOutputHelper output)
        : base(servers, playwright, output) { }

    [Fact]
    public async Task CampaignsList_ShowsSeededCampaign()
    {
        try
        {
            await Page.GotoAsync($"{BuilderUrl}/campaigns");

            await Expect(Page.GetByText("Lost Mine of Phandelver")).ToBeVisibleAsync();
            await Expect(Page.GetByText("A classic D&D 5e starter adventure.")).ToBeVisibleAsync();
        }
        catch { MarkFailed(); throw; }
    }

    [Fact]
    public async Task CampaignDetail_ShowsCampaignInfo()
    {
        try
        {
            await Page.GotoAsync($"{BuilderUrl}/campaigns/seed-campaign-1");

            // Wait for campaign to load
            await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Lost Mine of Phandelver" })).ToBeVisibleAsync();
            await Expect(Page.GetByText("A classic D&D 5e starter adventure.")).ToBeVisibleAsync();

            // Breadcrumb link back to campaigns
            await Expect(Page.Locator("nav[aria-label='breadcrumb'] a[href='/campaigns']")).ToBeVisibleAsync();
        }
        catch { MarkFailed(); throw; }
    }

    [Fact]
    public async Task CampaignDetail_ShowsStartSessionLink()
    {
        try
        {
            await Page.GotoAsync($"{BuilderUrl}/campaigns/seed-campaign-1");

            var startLink = Page.GetByRole(AriaRole.Link, new() { Name = "Start Session" });
            await Expect(startLink).ToBeVisibleAsync();

            // Verify the link points to the Player app with the campaignId
            var href = await startLink.GetAttributeAsync("href");
            Assert.NotNull(href);
            Assert.Contains("session/new", href);
            Assert.Contains("campaignId=seed-campaign-1", href);
        }
        catch { MarkFailed(); throw; }
    }

    [Fact]
    public async Task CampaignDetail_ShowsCharacters()
    {
        try
        {
            await Page.GotoAsync($"{BuilderUrl}/campaigns/seed-campaign-1");

            // Wait for characters section to load
            await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Characters" })).ToBeVisibleAsync();
        }
        catch { MarkFailed(); throw; }
    }

    private static ILocatorAssertions Expect(ILocator locator) => Assertions.Expect(locator);
}
