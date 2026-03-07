using MediatR;
using API.Services.Abstraction;
using Mediator.Mediator.Contracts.Session;
using Models.Session;
using Models.GameSystems;

namespace Mediator.Mediator.Handlers.Session;

public class PerformActionHandler(IGameSystemRegistry registry)
    : IRequestHandler<PerformActionRequest, SessionEvent>
{
    public Task<SessionEvent> Handle(PerformActionRequest request, CancellationToken cancellationToken)
    {
        if (!SessionStore.Sessions.TryGetValue(request.SessionId, out var state))
        {
            throw new KeyNotFoundException($"Session {request.SessionId} not found.");
        }

        var ruleBook = registry.Get(state.SystemId);
        SessionEvent sessionEvent;

        if (request.ActionType == ActionType.SkillCheck)
        {
            if (request.SkillCheckContext == null)
            {
                throw new ArgumentException("SkillCheckContext is required for SkillCheck action.");
            }
            
            var result = ruleBook.ResolveSkillCheck(request.SkillCheckContext);
            sessionEvent = new SessionEvent(
                "SkillCheck",
                $"Performed {request.SkillCheckContext.SkillId} check: {result.Message}",
                DateTime.UtcNow,
                CheckResult: result);
        }
        else // Attack
        {
            if (request.AttackContext == null)
            {
                throw new ArgumentException("AttackContext is required for Attack action.");
            }
            
            var result = ruleBook.ResolveAttack(request.AttackContext);
            sessionEvent = new SessionEvent(
                "Attack",
                $"Performed attack with {request.AttackContext.WeaponId}: {result.Message}",
                DateTime.UtcNow,
                AttackResult: result);
        }

        // Update session state
        var newEventLog = new List<SessionEvent>(state.EventLog) { sessionEvent };
        var newState = state with { EventLog = newEventLog };
        SessionStore.Sessions[request.SessionId] = newState;

        return Task.FromResult(sessionEvent);
    }
}
