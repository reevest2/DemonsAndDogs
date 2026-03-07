using MediatR;
using Mediator.Mediator.Contracts.Session;
using Models.Session;

namespace Mediator.Mediator.Handlers.Session;

public class GetSessionHandler : IRequestHandler<GetSessionRequest, SessionState>
{
    public Task<SessionState> Handle(GetSessionRequest request, CancellationToken cancellationToken)
    {
        if (SessionStore.Sessions.TryGetValue(request.SessionId, out var session))
        {
            return Task.FromResult(session);
        }
        
        throw new KeyNotFoundException($"Session {request.SessionId} not found.");
    }
}
