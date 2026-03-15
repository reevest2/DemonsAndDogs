using MediatR;
using API.Services.Sessions.Contracts;
using Models.Interfaces;
using Models.Session;

namespace API.Services.Sessions.Handlers;

public class GetSessionHandler(ISessionStore sessionStore, ISessionPersistence persistence)
    : IRequestHandler<GetSessionRequest, SessionState>
{
    public async Task<SessionState> Handle(GetSessionRequest request, CancellationToken cancellationToken)
    {
        if (sessionStore.TryGet(request.SessionId, out var session))
            return session!;

        var loaded = await persistence.LoadAsync(request.SessionId, cancellationToken);
        if (loaded == null)
            throw new KeyNotFoundException($"Session {request.SessionId} not found.");

        sessionStore.Set(request.SessionId, loaded);
        return loaded;
    }
}
