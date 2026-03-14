using Models.Interfaces;
using Models.Session;

namespace DemonsAndDogs.API.Tests.Fakes;

/// <summary>No-op ISessionPersistence for tests that don't exercise persistence.</summary>
public class NullSessionPersistence : ISessionPersistence
{
    public Task SaveAsync(SessionState state, CancellationToken ct) => Task.CompletedTask;
    public Task<SessionState?> LoadAsync(string sessionId, CancellationToken ct) => Task.FromResult<SessionState?>(null);
}
