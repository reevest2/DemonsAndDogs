using Bunit;
using Microsoft.Extensions.DependencyInjection;
using UIComponents.Shared;
using UIComponents.Services;
using Xunit;
using System.Linq;

namespace DemonsAndDogs.Tests.Components;

public class NavMenuTests : TestContext
{
    public NavMenuTests()
    {
        Services.AddSingleton<ThemeService>();
    }

    [Fact]
    public void NavMenu_ContainsHomeLink()
    {
        // Act
        var cut = Render<NavMenu>();
        // Assert
        var brandLink = cut.Find("a.brand");
        Assert.Equal("/", brandLink.GetAttribute("href"));
        Assert.Contains("Demons &amp; Dogs", brandLink.InnerHtml);
    }

    [Fact]
    public void NavMenu_ContainsNavigationLinks()
    {
        // Act
        var cut = Render<NavMenu>();
        // Assert
        var links = cut.FindAll("a.nav-link");
        
        var hrefs = links.Select(l => l.GetAttribute("href")).ToList();
        Assert.Contains("/campaigns", hrefs);
        Assert.Contains("/characters", hrefs);
        Assert.Contains("/systems", hrefs);
    }

    [Fact]
    public void NavMenu_ThemeToggle_ChangesTheme()
    {
        // Arrange
        var themeService = Services.GetRequiredService<ThemeService>();
        var cut = Render<NavMenu>();
        Assert.Equal("fantasy", themeService.CurrentTheme);

        // Act
        var button = cut.Find(".theme-toggle button");
        button.Click();

        // Assert
        Assert.Equal("steampunk", themeService.CurrentTheme);
        Assert.Contains("📜 Fantasy", button.InnerHtml); // Button text should change to the other theme option
    }
}
