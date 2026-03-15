using DemonsAndDogs.E2E.Tests.Fixtures;
using Microsoft.Playwright;
using Xunit.Abstractions;

namespace DemonsAndDogs.E2E.Tests.Player;

[Collection("E2E")]
public class HomeTests : E2ETestBase
{
    public HomeTests(ServerFixture servers, PlaywrightFixture playwright, ITestOutputHelper output)
        : base(servers, playwright, output) { }

    [Fact]
    public async Task HomePage_ShowsStartButton()
    {
        try
        {
            await Page.GotoAsync(PlayerUrl);

            await Expect(Page.GetByText("Demons & Dogs")).ToBeVisibleAsync();
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Start New Session" })).ToBeVisibleAsync();
        }
        catch { MarkFailed(); throw; }
    }

    [Fact]
    public async Task HomePage_StartButton_NavigatesToNewSession()
    {
        try
        {
            await Page.GotoAsync(PlayerUrl);

            await Page.GetByRole(AriaRole.Link, new() { Name = "Start New Session" }).ClickAsync();

            await Page.WaitForURLAsync("**/session/new");
            Assert.Contains("/session/new", Page.Url);
        }
        catch { MarkFailed(); throw; }
    }

    private static ILocatorAssertions Expect(ILocator locator) => Assertions.Expect(locator);
}
