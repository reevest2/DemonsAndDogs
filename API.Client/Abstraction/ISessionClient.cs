using Mediator.Mediator.Contracts.Session;
using Models.Session;

namespace API.Client.Abstraction;

public interface ISessionClient
{
    Task<SessionState> StartSessionAsync(StartSessionRequest request, CancellationToken ct = default);
    Task<SessionEvent> PerformActionAsync(PerformActionRequest request, CancellationToken ct = default);
    Task<SessionState> GetSessionAsync(string sessionId, CancellationToken ct = default);
}
