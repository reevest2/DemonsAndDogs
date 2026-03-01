using System.Text.RegularExpressions;
using Microsoft.Playwright.NUnit;

namespace DemonsAndDogs.PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class HomePageTests : PageTest
{
    private const string BaseUrl = "http://localhost:5150";

    [Test]
    public async Task HomePage_ShouldLoad()
    {
        await Page.GotoAsync(BaseUrl);

        await Expect(Page).ToHaveTitleAsync(new Regex(".*"));
        Assert.That(Page.Url, Does.Contain(BaseUrl));
    }

    [Test]
    public async Task HomePage_ShouldHaveNavigation()
    {
        await Page.GotoAsync(BaseUrl);

        var nav = Page.Locator("nav");
        await Expect(nav).ToBeVisibleAsync();
    }

    [Test]
    public async Task HomePage_ShouldDisplayContent()
    {
        await Page.GotoAsync(BaseUrl);

        var body = Page.Locator("body");
        await Expect(body).ToBeVisibleAsync();
    }
}
