using System.Collections.Concurrent;
using Models.Interfaces;
using Models.Session;

namespace API.Services.Sessions;

/// <summary>
/// In-memory session store for MVP. Register as Singleton in DI.
/// </summary>
public class SessionStore : ISessionStore
{
    private readonly ConcurrentDictionary<string, SessionState> _sessions = new();

    public bool TryGet(string sessionId, out SessionState? state) =>
        _sessions.TryGetValue(sessionId, out state);

    public void Set(string sessionId, SessionState state) =>
        _sessions[sessionId] = state;
}
