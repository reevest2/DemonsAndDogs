using DemonsAndDogs.E2E.Tests.Fixtures;
using Microsoft.Playwright;
using Xunit.Abstractions;

namespace DemonsAndDogs.E2E.Tests.Player;

[Collection("E2E")]
public class NewSessionTests : E2ETestBase
{
    public NewSessionTests(ServerFixture servers, PlaywrightFixture playwright, ITestOutputHelper output)
        : base(servers, playwright, output) { }

    [Fact]
    public async Task NewSession_DropdownsPopulated()
    {
        try
        {
            await Page.GotoAsync($"{PlayerUrl}/session/new");

            // Wait for the form to load
            await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Start New Session" })).ToBeVisibleAsync();

            // Campaign dropdown should have the seeded campaign
            var campaignSelect = Page.Locator("select").First;
            await Expect(campaignSelect.Locator("option", new() { HasText = "Lost Mine of Phandelver" })).ToBeAttachedAsync();

            // Character dropdown should have seeded characters
            var characterSelect = Page.Locator("select").Nth(1);
            await Expect(characterSelect.Locator("option", new() { HasText = "Gimli" })).ToBeAttachedAsync();
            await Expect(characterSelect.Locator("option", new() { HasText = "Legolas" })).ToBeAttachedAsync();
        }
        catch { MarkFailed(); throw; }
    }

    [Fact]
    public async Task NewSession_StartButton_DisabledUntilBothSelected()
    {
        try
        {
            await Page.GotoAsync($"{PlayerUrl}/session/new");

            await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Start New Session" })).ToBeVisibleAsync();

            // Button should be disabled initially
            var startButton = Page.GetByRole(AriaRole.Button, new() { Name = "Start Session" });
            await Expect(startButton).ToBeDisabledAsync();

            // Select campaign
            await Page.Locator("select").First.SelectOptionAsync(new SelectOptionValue { Label = "Lost Mine of Phandelver" });

            // Still disabled — character not selected
            await Expect(startButton).ToBeDisabledAsync();

            // Select character
            await Page.Locator("select").Nth(1).SelectOptionAsync(new SelectOptionValue { Label = "Gimli (dnd5e)" });

            // Now enabled
            await Expect(startButton).ToBeEnabledAsync();
        }
        catch { MarkFailed(); throw; }
    }

    [Fact]
    public async Task NewSession_QueryParam_PreselectsCampaign()
    {
        try
        {
            await Page.GotoAsync($"{PlayerUrl}/session/new?campaignId=seed-campaign-1");

            await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Start New Session" })).ToBeVisibleAsync();

            // Campaign dropdown should have the seeded campaign pre-selected
            var campaignSelect = Page.Locator("select").First;
            var selectedValue = await campaignSelect.InputValueAsync();
            Assert.Equal("seed-campaign-1", selectedValue);
        }
        catch { MarkFailed(); throw; }
    }

    private static ILocatorAssertions Expect(ILocator locator) => Assertions.Expect(locator);
}
