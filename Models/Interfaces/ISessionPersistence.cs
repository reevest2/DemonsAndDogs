using Models.Session;

namespace Models.Interfaces;

public interface ISessionPersistence
{
    Task SaveAsync(SessionState state, CancellationToken ct);
    Task<SessionState?> LoadAsync(string sessionId, CancellationToken ct);
}
