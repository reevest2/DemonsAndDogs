using System.Net;
using System.Net.Http.Json;
using AppConstants;
using DemonsAndDogs.API.Tests.Startup;
using API.Services.Sessions.Contracts;
using Models.GameSystems;
using Models.Session;

namespace DemonsAndDogs.API.Tests.Controllers;

/// <summary>
/// Integration tests for the Session API endpoints.
/// Sessions are the core gameplay flow: start → perform actions → get state.
/// Uses seeded characters (Gimli, Legolas) and the DnD5e game system.
/// </summary>
public class SessionControllerTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client;

    public SessionControllerTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // -----------------------------------------------------------------------
    // POST /api/session/start
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Start_ValidRequest_ReturnsOk()
    {
        var request = new StartSessionRequest("char-1", "Gimli", "dnd5e");

        var response = await _client.PostAsJsonAsync("/api/session/start", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Start_ValidRequest_ReturnsSessionStateWithSessionId()
    {
        var request = new StartSessionRequest("char-1", "Gimli", "dnd5e");

        var state = await (await _client.PostAsJsonAsync("/api/session/start", request))
            .Content.ReadFromJsonAsync<SessionState>();

        Assert.NotNull(state);
        Assert.False(string.IsNullOrEmpty(state.SessionId));
        Assert.Equal("Gimli", state.CharacterName);
        Assert.Equal("dnd5e", state.SystemId);
    }

    [Fact]
    public async Task Start_ValidRequest_ReturnsSessionWithSchema()
    {
        var request = new StartSessionRequest("char-1", "Gimli", "dnd5e");

        var state = await (await _client.PostAsJsonAsync("/api/session/start", request))
            .Content.ReadFromJsonAsync<SessionState>();

        Assert.NotNull(state!.CharacterSheetSchema);
        Assert.Equal("dnd5e", state.CharacterSheetSchema.SystemId);
    }

    [Fact]
    public async Task Start_ValidRequest_ReturnsSessionWithEmptyEventLog()
    {
        var request = new StartSessionRequest("char-1", "Gimli", "dnd5e");

        var state = await (await _client.PostAsJsonAsync("/api/session/start", request))
            .Content.ReadFromJsonAsync<SessionState>();

        Assert.NotNull(state);
        Assert.Empty(state.EventLog);
    }

    // -----------------------------------------------------------------------
    // POST /api/session/action
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Action_SkillCheck_ReturnsSessionEvent()
    {
        // Start a session first
        var startReq = new StartSessionRequest("char-1", "Gimli", "dnd5e");
        var state = await (await _client.PostAsJsonAsync("/api/session/start", startReq))
            .Content.ReadFromJsonAsync<SessionState>();

        var actionReq = new PerformActionRequest(
            state!.SessionId,
            ActionType.SkillCheck,
            new SkillCheckContext("char-1", "stealth", 0, 0, 10));

        var response = await _client.PostAsJsonAsync("/api/session/action", actionReq);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var sessionEvent = await response.Content.ReadFromJsonAsync<SessionEvent>();
        Assert.NotNull(sessionEvent);
        Assert.Equal("SkillCheck", sessionEvent.EventType);
    }

    [Fact]
    public async Task Action_Attack_ReturnsSessionEvent()
    {
        var startReq = new StartSessionRequest("char-1", "Gimli", "dnd5e");
        var state = await (await _client.PostAsJsonAsync("/api/session/start", startReq))
            .Content.ReadFromJsonAsync<SessionState>();

        var actionReq = new PerformActionRequest(
            state!.SessionId,
            ActionType.Attack,
            AttackContext: new AttackContext("sword", 0, 15));

        var response = await _client.PostAsJsonAsync("/api/session/action", actionReq);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var sessionEvent = await response.Content.ReadFromJsonAsync<SessionEvent>();
        Assert.NotNull(sessionEvent);
        Assert.Equal("Attack", sessionEvent.EventType);
    }

    // -----------------------------------------------------------------------
    // GET /api/session/{sessionId}
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetSession_AfterStartAndAction_ReturnsStateWithEventLog()
    {
        // Start session
        var startReq = new StartSessionRequest("char-1", "Gimli", "dnd5e");
        var state = await (await _client.PostAsJsonAsync("/api/session/start", startReq))
            .Content.ReadFromJsonAsync<SessionState>();

        // Perform an action
        var actionReq = new PerformActionRequest(
            state!.SessionId,
            ActionType.SkillCheck,
            new SkillCheckContext("char-1", "stealth", 0, 0, 10));
        await _client.PostAsJsonAsync("/api/session/action", actionReq);

        // Get session state
        var response = await _client.GetAsync($"/api/session/{state.SessionId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var loaded = await response.Content.ReadFromJsonAsync<SessionState>();
        Assert.NotNull(loaded);
        Assert.Equal(state.SessionId, loaded.SessionId);
        Assert.Single(loaded.EventLog);
    }

    // -----------------------------------------------------------------------
    // Full Flow: Start → Action → Action → Get Session
    // -----------------------------------------------------------------------

    [Fact]
    public async Task FullFlow_StartThenMultipleActions_EventLogGrows()
    {
        // Start
        var state = await (await _client.PostAsJsonAsync("/api/session/start",
            new StartSessionRequest("char-1", "Gimli", "dnd5e")))
            .Content.ReadFromJsonAsync<SessionState>();

        // Skill check
        await _client.PostAsJsonAsync("/api/session/action",
            new PerformActionRequest(state!.SessionId, ActionType.SkillCheck,
                new SkillCheckContext("char-1", "perception", 0, 0, 12)));

        // Attack
        await _client.PostAsJsonAsync("/api/session/action",
            new PerformActionRequest(state.SessionId, ActionType.Attack,
                AttackContext: new AttackContext("axe", 0, 13)));

        // Verify
        var loaded = await _client.GetFromJsonAsync<SessionState>($"/api/session/{state.SessionId}");
        Assert.Equal(2, loaded!.EventLog.Count);
        Assert.Equal("SkillCheck", loaded.EventLog[0].EventType);
        Assert.Equal("Attack", loaded.EventLog[1].EventType);
    }
}
