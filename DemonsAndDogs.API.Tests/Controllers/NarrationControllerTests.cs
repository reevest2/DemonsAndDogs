using System.Net;
using System.Net.Http.Json;
using AppConstants;
using API.Services.Sessions.Contracts;
using Models.GameSystems;
using Models.Interfaces;
using Models.Narration;
using Models.Session;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DemonsAndDogs.API.Tests.Controllers;

// ---------------------------------------------------------------------------
// Test-specific factory that replaces the real narrator with a fake
// ---------------------------------------------------------------------------

public class NarrationTestFactory : WebApplicationFactory<global::API.Program>
{
    private readonly string _dbName = $"TestDb_{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            // Unique in-memory DB to avoid cross-test contamination
            var dbDescriptor = services.FirstOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<DataAccess.DbContext>));
            if (dbDescriptor != null) services.Remove(dbDescriptor);

            services.AddDbContext<DataAccess.DbContext>(options =>
                options.UseInMemoryDatabase(_dbName));

            // Remove the real INarrator registration and replace with our fake
            var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(INarrator));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddScoped<INarrator, FakeNarrator>();
        });
    }
}

/// <summary>
/// Fake narrator that returns a simple text response (no LM Studio dependency).
/// </summary>
public class FakeNarrator : INarrator
{
    public Task<NarrationResult> NarrateAsync(GameEvent gameEvent, string? tone = null, CancellationToken cancellationToken = default)
    {
        var tokens = StreamTokens("The brave hero ", "ventured ", "forth.");
        return Task.FromResult(new NarrationResult("The brave hero ventured forth.", TokenStream: tokens));
    }

    private static async IAsyncEnumerable<string> StreamTokens(params string[] tokens)
    {
        foreach (var token in tokens)
        {
            await Task.Yield();
            yield return token;
        }
    }
}

/// <summary>
/// Integration tests for the Narration API endpoint.
/// Uses a fake narrator to avoid LM Studio dependency.
/// The narration endpoint streams Server-Sent Events (SSE).
/// </summary>
public class NarrationControllerTests : IClassFixture<NarrationTestFactory>
{
    private readonly HttpClient _client;

    public NarrationControllerTests(NarrationTestFactory factory)
    {
        _client = factory.CreateClient();
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private async Task<string> StartSessionAndPerformAction()
    {
        var startReq = new StartSessionRequest("char-1", "Gimli", "dnd5e");
        var state = await (await _client.PostAsJsonAsync("/api/session/start", startReq))
            .Content.ReadFromJsonAsync<SessionState>();

        var actionReq = new PerformActionRequest(
            state!.SessionId,
            ActionType.SkillCheck,
            new SkillCheckContext("char-1", "stealth", 0, 0, 10));
        await _client.PostAsJsonAsync("/api/session/action", actionReq);

        return state.SessionId;
    }

    // -----------------------------------------------------------------------
    // GET /api/narration/stream/{sessionId}
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetNarrationStream_AfterAction_ReturnsSSEContentType()
    {
        var sessionId = await StartSessionAndPerformAction();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/narration/stream/{sessionId}");
        var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/event-stream", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetNarrationStream_AfterAction_StreamContainsSSEDataLines()
    {
        var sessionId = await StartSessionAndPerformAction();

        var response = await _client.GetAsync($"/api/narration/stream/{sessionId}");
        var content = await response.Content.ReadAsStringAsync();

        // SSE format: "data: <token>\n\n"
        Assert.Contains("data: ", content);
    }

    [Fact]
    public async Task GetNarrationStream_AfterAction_StreamContainsNarrationTokens()
    {
        var sessionId = await StartSessionAndPerformAction();

        var response = await _client.GetAsync($"/api/narration/stream/{sessionId}");
        var content = await response.Content.ReadAsStringAsync();

        // Our FakeNarrator streams "The brave hero ", "ventured ", "forth."
        Assert.Contains("The brave hero", content);
        Assert.Contains("ventured", content);
        Assert.Contains("forth", content);
    }
}
