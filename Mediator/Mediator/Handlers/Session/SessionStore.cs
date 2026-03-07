using System.Collections.Concurrent;
using Models.Session;

namespace Mediator.Mediator.Handlers.Session;

/// <summary>
/// Simple in-memory session store for MVP.
/// </summary>
public static class SessionStore
{
    public static ConcurrentDictionary<string, SessionState> Sessions { get; } = new();
}
