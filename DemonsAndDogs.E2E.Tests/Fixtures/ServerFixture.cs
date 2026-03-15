using DemonsAndDogs.E2E.Tests.Helpers;

namespace DemonsAndDogs.E2E.Tests.Fixtures;

public class ServerFixture : IAsyncLifetime
{
    private readonly ServerProcessManager _api = new("API");
    private readonly ServerProcessManager _builder = new("Builder");
    private readonly ServerProcessManager _player = new("Player");

    public string ApiBaseUrl => "https://localhost:44390";
    public string BuilderBaseUrl => "http://localhost:5150";
    public string PlayerBaseUrl => "http://localhost:5160";

    // Relative to the test output directory (bin/Debug/net10.0)
    private static string SolutionRoot => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

    public async Task InitializeAsync()
    {
        // Build once before starting all servers
        var buildProcess = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"build \"{Path.Combine(SolutionRoot, "DemonsAndDogs.sln")}\" --verbosity minimal",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        });
        if (buildProcess != null)
        {
            await buildProcess.WaitForExitAsync();
            if (buildProcess.ExitCode != 0)
                throw new InvalidOperationException("Solution build failed");
        }

        // Start all three servers
        await _api.StartAsync(
            Path.Combine(SolutionRoot, "API", "API.csproj"),
            "Testing",
            ApiBaseUrl);

        await _builder.StartAsync(
            Path.Combine(SolutionRoot, "DemonsAndDogs.Builder", "DemonsAndDogs.Builder.csproj"),
            "Development",
            BuilderBaseUrl);

        await _player.StartAsync(
            Path.Combine(SolutionRoot, "DemonsAndDogs.Player", "DemonsAndDogs.Player.csproj"),
            "Development",
            PlayerBaseUrl);

        // Wait for all servers to be ready
        await Task.WhenAll(
            _api.WaitForReadyAsync($"{ApiBaseUrl}/swagger/v1/swagger.json", timeoutSeconds: 30),
            _builder.WaitForReadyAsync(BuilderBaseUrl, timeoutSeconds: 60),
            _player.WaitForReadyAsync(PlayerBaseUrl, timeoutSeconds: 60));
    }

    public async Task DisposeAsync()
    {
        await _api.DisposeAsync();
        await _builder.DisposeAsync();
        await _player.DisposeAsync();
    }
}
