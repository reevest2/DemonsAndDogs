using MediatR;
using API.Services.Abstraction;
using Mediator.Mediator.Contracts.Session;
using Models.Session;
using Models.GameSystems;

namespace Mediator.Mediator.Handlers.Session;

public class StartSessionHandler(IGameSystemRegistry registry)
    : IRequestHandler<StartSessionRequest, SessionState>
{
    public Task<SessionState> Handle(StartSessionRequest request, CancellationToken cancellationToken)
    {
        var ruleBook = registry.Get(request.SystemId);
        var schema = ruleBook.GetCharacterSheetSchema();
        
        var sessionId = Guid.NewGuid().ToString();
        var state = new SessionState(
            sessionId,
            request.CharacterName,
            request.SystemId,
            schema,
            new List<SessionEvent>());
            
        SessionStore.Sessions[sessionId] = state;
        
        return Task.FromResult(state);
    }
}
