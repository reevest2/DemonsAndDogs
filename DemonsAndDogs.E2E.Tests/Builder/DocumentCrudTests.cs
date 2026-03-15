using DemonsAndDogs.E2E.Tests.Fixtures;
using Microsoft.Playwright;
using Xunit.Abstractions;

namespace DemonsAndDogs.E2E.Tests.Builder;

[Collection("E2E")]
public class DocumentCrudTests : E2ETestBase
{
    public DocumentCrudTests(ServerFixture servers, PlaywrightFixture playwright, ITestOutputHelper output)
        : base(servers, playwright, output) { }

    [Fact]
    public async Task CreateDocument_AppearsInList()
    {
        try
        {
            await Page.GotoAsync($"{BuilderUrl}/campaigns/seed-campaign-1");
            await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Documents" })).ToBeVisibleAsync();

            // Click "+ New Document"
            await Page.GetByRole(AriaRole.Button, new() { Name = "+ New Document" }).ClickAsync();

            // Step 1: select NPC category
            await Page.GetByText("NPC").ClickAsync();

            // Step 2: fill in the form
            await Page.Locator("input[placeholder='Enter title...']").FillAsync("Test NPC Bartender");
            await Page.Locator("input[placeholder*='Human']").FillAsync("Human");
            await Page.Locator("input[placeholder*='Blacksmith']").FillAsync("Tavern Keeper");

            // Save
            await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

            // Verify document appears in list
            await Expect(Page.GetByText("Test NPC Bartender")).ToBeVisibleAsync();
            await Expect(Page.GetByText("NPC", new() { Exact = true })).ToBeVisibleAsync();
        }
        catch { MarkFailed(); throw; }
    }

    [Fact]
    public async Task CreateAndDeleteDocument_RemovesFromList()
    {
        try
        {
            await Page.GotoAsync($"{BuilderUrl}/campaigns/seed-campaign-1");
            await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Documents" })).ToBeVisibleAsync();

            // Create a document first
            await Page.GetByRole(AriaRole.Button, new() { Name = "+ New Document" }).ClickAsync();
            await Page.GetByText("Lore").ClickAsync();
            await Page.Locator("input[placeholder='Enter title...']").FillAsync("Deletable Lore Entry");
            await Page.Locator("input[placeholder*='History']").FillAsync("Legend");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

            // Verify it exists
            await Expect(Page.GetByText("Deletable Lore Entry")).ToBeVisibleAsync();

            // Click Delete on that document's row
            var docRow = Page.Locator(".card", new() { HasText = "Deletable Lore Entry" }).First;
            await docRow.GetByRole(AriaRole.Button, new() { Name = "Delete" }).ClickAsync();

            // Confirm deletion
            await Page.GetByRole(AriaRole.Button, new() { Name = "Delete" }).First.ClickAsync();

            // Verify removed
            await Expect(Page.GetByText("Deletable Lore Entry")).Not.ToBeVisibleAsync();
        }
        catch { MarkFailed(); throw; }
    }

    private static ILocatorAssertions Expect(ILocator locator) => Assertions.Expect(locator);
}
