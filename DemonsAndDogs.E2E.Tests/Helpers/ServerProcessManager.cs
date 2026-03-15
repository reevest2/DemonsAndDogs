using System.Diagnostics;
using System.Net.Http;

namespace DemonsAndDogs.E2E.Tests.Helpers;

public class ServerProcessManager : IAsyncDisposable
{
    private Process? _process;
    private readonly string _name;

    public ServerProcessManager(string name)
    {
        _name = name;
    }

    public async Task StartAsync(string projectPath, string environment, string urls)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project \"{projectPath}\" --no-build",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        startInfo.Environment["ASPNETCORE_ENVIRONMENT"] = environment;
        startInfo.Environment["ASPNETCORE_URLS"] = urls;

        _process = Process.Start(startInfo);
        if (_process == null)
            throw new InvalidOperationException($"Failed to start {_name} process");

        // Consume output to prevent buffer deadlock
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();
    }

    public async Task WaitForReadyAsync(string healthUrl, int timeoutSeconds = 60)
    {
        using var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
        using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(5) };

        var deadline = DateTime.UtcNow.AddSeconds(timeoutSeconds);
        while (DateTime.UtcNow < deadline)
        {
            try
            {
                var response = await client.GetAsync(healthUrl);
                if (response.IsSuccessStatusCode)
                    return;
            }
            catch
            {
                // Server not ready yet
            }

            if (_process?.HasExited == true)
                throw new InvalidOperationException($"{_name} process exited unexpectedly with code {_process.ExitCode}");

            await Task.Delay(1000);
        }

        throw new TimeoutException($"{_name} did not become ready within {timeoutSeconds} seconds at {healthUrl}");
    }

    public async ValueTask DisposeAsync()
    {
        if (_process != null && !_process.HasExited)
        {
            try
            {
                _process.Kill(entireProcessTree: true);
                await _process.WaitForExitAsync();
            }
            catch
            {
                // Best effort cleanup
            }
        }
        _process?.Dispose();
    }
}
