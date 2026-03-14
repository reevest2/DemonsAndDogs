using MediatR;
using API.Services.Abstraction;
using Mediator.Mediator.Contracts.Session;
using Models.Interfaces;
using Models.Session;
using Models.GameSystems;

namespace Mediator.Mediator.Handlers.Session;

public class StartSessionHandler(IGameSystemRegistry registry, ISessionStore sessionStore, ISessionPersistence persistence)
    : IRequestHandler<StartSessionRequest, SessionState>
{
    public async Task<SessionState> Handle(StartSessionRequest request, CancellationToken cancellationToken)
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

        sessionStore.Set(sessionId, state);
        await persistence.SaveAsync(state, cancellationToken);

        return state;
    }
}
