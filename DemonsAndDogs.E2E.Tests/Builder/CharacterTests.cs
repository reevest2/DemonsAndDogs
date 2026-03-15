using DemonsAndDogs.E2E.Tests.Fixtures;
using Microsoft.Playwright;
using Xunit.Abstractions;

namespace DemonsAndDogs.E2E.Tests.Builder;

[Collection("E2E")]
public class CharacterTests : E2ETestBase
{
    public CharacterTests(ServerFixture servers, PlaywrightFixture playwright, ITestOutputHelper output)
        : base(servers, playwright, output) { }

    [Fact]
    public async Task CharactersList_ShowsSeededCharacters()
    {
        try
        {
            await Page.GotoAsync($"{BuilderUrl}/characters");

            await Expect(Page.GetByText("Gimli")).ToBeVisibleAsync();
            await Expect(Page.GetByText("Legolas")).ToBeVisibleAsync();
        }
        catch { MarkFailed(); throw; }
    }

    [Fact]
    public async Task CharacterDetail_ShowsCharacterInfo()
    {
        try
        {
            await Page.GotoAsync($"{BuilderUrl}/characters/seed-char-1");

            // Character name should be visible
            await Expect(Page.GetByText("Gimli")).ToBeVisibleAsync();
        }
        catch { MarkFailed(); throw; }
    }

    private static ILocatorAssertions Expect(ILocator locator) => Assertions.Expect(locator);
}
