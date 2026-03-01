using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace DemonsAndDogs.PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class CreateSchemaPageTests : PageTest
{
    private const string BaseUrl = "http://localhost:5150";

    [Test]
    public async Task CreateSchemaPage_ShouldNavigateFromNav()
    {
        await Page.GotoAsync(BaseUrl);

        var link = Page.GetByRole(AriaRole.Link, new() { Name = "Create Schema" });
        await Expect(link).ToBeVisibleAsync();

        await link.ClickAsync();
        await Page.WaitForURLAsync("**/create-schema");

        Assert.That(Page.Url, Does.Contain("create-schema"));
    }

    [Test]
    public async Task CreateSchemaPage_ShouldLoadDirectly()
    {
        await Page.GotoAsync($"{BaseUrl}/create-schema");

        var body = Page.Locator("body");
        await Expect(body).ToBeVisibleAsync();
    }

    [Test]
    public async Task CreateSchemaPage_ShouldHaveOwnerIdInput()
    {
        await Page.GotoAsync($"{BaseUrl}/create-schema");

        var ownerInput = Page.GetByPlaceholder("Owner");
        await Expect(ownerInput).ToBeVisibleAsync();
    }

    [Test]
    public async Task CreateSchemaPage_ShouldHaveAddSectionButton()
    {
        await Page.GotoAsync($"{BaseUrl}/create-schema");

        var addButton = Page.GetByRole(AriaRole.Button, new() { Name = "Add Section" });
        await Expect(addButton).ToBeVisibleAsync();
    }
}
