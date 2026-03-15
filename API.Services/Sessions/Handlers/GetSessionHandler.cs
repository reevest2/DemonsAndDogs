using MediatR;
using API.Services.Sessions.Contracts;
using Models;
using Models.Interfaces;
using Models.Session;

namespace API.Services.Sessions.Handlers;

public class GetSessionHandler(ISessionStore sessionStore, ISessionPersistence persistence)
    : IRequestHandler<GetSessionRequest, Result<SessionState>>
{
    public async Task<Result<SessionState>> Handle(GetSessionRequest request, CancellationToken cancellationToken)
    {
        if (sessionStore.TryGet(request.SessionId, out var session))
            return Result<SessionState>.Ok(session!);

        var loaded = await persistence.LoadAsync(request.SessionId, cancellationToken);
        if (loaded == null)
            return Result<SessionState>.NotFound("Session", request.SessionId);

        sessionStore.Set(request.SessionId, loaded);
        return Result<SessionState>.Ok(loaded);
    }
}
