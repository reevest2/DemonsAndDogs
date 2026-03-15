using API.Client.Abstraction;
using API.Services.Sessions.Contracts;
using Models.Session;

namespace API.Client;

public class SessionClient(IApiClient apiClient) : ISessionClient
{
    public async Task<SessionState> StartSessionAsync(StartSessionRequest request, CancellationToken ct = default)
    {
        return await apiClient.Post<StartSessionRequest, SessionState>("api/session/start", request, ct);
    }

    public async Task<SessionEvent> PerformActionAsync(PerformActionRequest request, CancellationToken ct = default)
    {
        return await apiClient.Post<PerformActionRequest, SessionEvent>("api/session/action", request, ct);
    }

    public async Task<SessionState> GetSessionAsync(string sessionId, CancellationToken ct = default)
    {
        return await apiClient.Get<SessionState>($"api/session/{sessionId}", ct);
    }
}
