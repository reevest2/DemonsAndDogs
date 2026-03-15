using DemonsAndDogs.E2E.Tests.Fixtures;
using Microsoft.Playwright;
using Xunit.Abstractions;

namespace DemonsAndDogs.E2E.Tests.Player;

[Collection("E2E")]
public class SessionTests : E2ETestBase
{
    public SessionTests(ServerFixture servers, PlaywrightFixture playwright, ITestOutputHelper output)
        : base(servers, playwright, output) { }

    [Fact]
    public async Task StartSession_LoadsSessionPage()
    {
        try
        {
            await Page.GotoAsync($"{PlayerUrl}/session/new");
            await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Start New Session" })).ToBeVisibleAsync();

            // Select campaign and character
            await Page.Locator("select").First.SelectOptionAsync(new SelectOptionValue { Label = "Lost Mine of Phandelver" });
            await Page.Locator("select").Nth(1).SelectOptionAsync(new SelectOptionValue { Label = "Gimli (dnd5e)" });

            // Click Start Session
            await Page.GetByRole(AriaRole.Button, new() { Name = "Start Session" }).ClickAsync();

            // Should navigate to session page
            await Page.WaitForURLAsync("**/session/**");
            Assert.Contains("/session/", Page.Url);
            Assert.DoesNotContain("/session/new", Page.Url);
        }
        catch { MarkFailed(); throw; }
    }

    [Fact]
    public async Task ActiveSession_ShowsActionButtons()
    {
        try
        {
            // Start a session first
            await StartSessionAsync();

            // Verify action buttons are visible
            await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Roll Stealth" })).ToBeVisibleAsync();
            await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Attack" })).ToBeVisibleAsync();

            // Verify character name is shown
            await Expect(Page.GetByText("Gimli")).ToBeVisibleAsync();
        }
        catch { MarkFailed(); throw; }
    }

    [Fact]
    public async Task ActiveSession_ActionButton_UpdatesLog()
    {
        try
        {
            await StartSessionAsync();

            // Click Roll Stealth
            await Page.GetByRole(AriaRole.Button, new() { Name = "Roll Stealth" }).ClickAsync();

            // Wait for the action log to update — look for "Stealth" in the event log
            await Expect(Page.GetByText("Stealth", new() { Exact = false })).ToBeVisibleAsync();
        }
        catch { MarkFailed(); throw; }
    }

    private async Task StartSessionAsync()
    {
        await Page.GotoAsync($"{PlayerUrl}/session/new");
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Start New Session" })).ToBeVisibleAsync();

        await Page.Locator("select").First.SelectOptionAsync(new SelectOptionValue { Label = "Lost Mine of Phandelver" });
        await Page.Locator("select").Nth(1).SelectOptionAsync(new SelectOptionValue { Label = "Gimli (dnd5e)" });
        await Page.GetByRole(AriaRole.Button, new() { Name = "Start Session" }).ClickAsync();
        await Page.WaitForURLAsync("**/session/**");
    }

    private static ILocatorAssertions Expect(ILocator locator) => Assertions.Expect(locator);
}
