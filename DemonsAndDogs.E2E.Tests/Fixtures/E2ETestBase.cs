using Microsoft.Playwright;
using Xunit.Abstractions;

namespace DemonsAndDogs.E2E.Tests.Fixtures;

[Collection("E2E")]
public abstract class E2ETestBase : IAsyncLifetime
{
    protected readonly ServerFixture Servers;
    protected readonly PlaywrightFixture PlaywrightFixture;
    protected readonly ITestOutputHelper Output;

    protected IBrowserContext Context { get; private set; } = null!;
    protected IPage Page { get; private set; } = null!;

    protected string BuilderUrl => Servers.BuilderBaseUrl;
    protected string PlayerUrl => Servers.PlayerBaseUrl;
    protected string ApiUrl => Servers.ApiBaseUrl;

    private static readonly string ScreenshotDir = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "TestResults", "screenshots");

    private bool _failed;

    protected E2ETestBase(ServerFixture servers, PlaywrightFixture playwright, ITestOutputHelper output)
    {
        Servers = servers;
        PlaywrightFixture = playwright;
        Output = output;
    }

    public async Task InitializeAsync()
    {
        Context = await PlaywrightFixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true
        });
        Page = await Context.NewPageAsync();
        Page.SetDefaultTimeout(30_000);
    }

    public async Task DisposeAsync()
    {
        // xUnit doesn't expose test result in IAsyncLifetime, so we rely on
        // manual ScreenshotAsync calls or the MarkFailed pattern.
        if (_failed)
        {
            await CaptureScreenshotAsync("failure");
        }

        await Context.CloseAsync();
    }

    /// <summary>
    /// Call this in a catch block or at any point to capture a named screenshot.
    /// </summary>
    protected async Task ScreenshotAsync(string label)
    {
        await CaptureScreenshotAsync(label);
    }

    /// <summary>
    /// Mark the test as failed so a screenshot is captured on dispose.
    /// Use in catch blocks: catch { MarkFailed(); throw; }
    /// </summary>
    protected void MarkFailed() => _failed = true;

    private async Task CaptureScreenshotAsync(string label)
    {
        try
        {
            Directory.CreateDirectory(ScreenshotDir);
            var className = GetType().Name;
            var timestamp = DateTime.Now.ToString("HHmmss");
            var fileName = $"{className}_{label}_{timestamp}.png";
            var path = Path.Combine(ScreenshotDir, fileName);

            await Page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = path,
                FullPage = true
            });

            Output.WriteLine($"Screenshot saved: {path}");
        }
        catch (Exception ex)
        {
            Output.WriteLine($"Failed to capture screenshot: {ex.Message}");
        }
    }
}
