using Models.Session;

namespace Models.Interfaces;

public interface ISessionStore
{
    bool TryGet(string sessionId, out SessionState? state);
    void Set(string sessionId, SessionState state);
}
