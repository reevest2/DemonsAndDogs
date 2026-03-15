using MediatR;
using API.Services.Characters;
using API.Services.GameSystems;
using API.Services.Sessions;
using API.Services.Sessions.Contracts;
using Models.Interfaces;
using Models.Session;
using Models.GameSystems;

namespace API.Services.Sessions.Handlers;

public class StartSessionHandler(IGameSystemRegistry registry, ISessionStore sessionStore, ISessionPersistence persistence, ICharacterService characterService)
    : IRequestHandler<StartSessionRequest, SessionState>
{
    public async Task<SessionState> Handle(StartSessionRequest request, CancellationToken cancellationToken)
    {
        var ruleBook = registry.Get(request.SystemId);
        var schema = ruleBook.GetCharacterSheetSchema();

        var character = await characterService.GetByIdAsync(request.CharacterId, cancellationToken);
        var stats = character != null
            ? ruleBook.ExtractStats(character.Data)
            : (IReadOnlyDictionary<string, int>)new Dictionary<string, int>();

        var sessionId = Guid.NewGuid().ToString();
        var state = new SessionState(
            sessionId,
            request.CharacterName,
            request.SystemId,
            schema,
            stats,
            new List<SessionEvent>());

        sessionStore.Set(sessionId, state);
        await persistence.SaveAsync(state, cancellationToken);

        return state;
    }
}
