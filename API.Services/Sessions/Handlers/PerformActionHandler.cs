using AppConstants;
using MediatR;
using API.Services.GameSystems;
using API.Services.Sessions;
using API.Services.Sessions.Contracts;
using Models;
using Models.Interfaces;
using Models.Session;
using Models.GameSystems;

namespace API.Services.Sessions.Handlers;

public class PerformActionHandler(IGameSystemRegistry registry, ISessionStore sessionStore, ISessionPersistence persistence)
    : IRequestHandler<PerformActionRequest, Result<SessionEvent>>
{
    public async Task<Result<SessionEvent>> Handle(PerformActionRequest request, CancellationToken cancellationToken)
    {
        if (!sessionStore.TryGet(request.SessionId, out var state))
            return Result<SessionEvent>.NotFound("Session", request.SessionId);

        var ruleBookResult = registry.Get(state!.SystemId);
        if (!ruleBookResult.IsSuccess)
            return Result<SessionEvent>.Fail(ruleBookResult.Error!);

        var ruleBook = ruleBookResult.Value!;
        SessionEvent sessionEvent;

        if (request.ActionType == ActionType.SkillCheck)
        {
            if (request.SkillCheckContext == null)
                return Result<SessionEvent>.InvalidInput("SkillCheckContext is required for SkillCheck action.");

            var result = ruleBook.ResolveSkillCheck(request.SkillCheckContext);
            sessionEvent = new SessionEvent(
                ActionEventTypes.SkillCheck,
                $"Performed {request.SkillCheckContext.SkillId} check: {result.Message}",
                DateTime.UtcNow,
                CheckResult: result);
        }
        else // Attack
        {
            if (request.AttackContext == null)
                return Result<SessionEvent>.InvalidInput("AttackContext is required for Attack action.");

            var result = ruleBook.ResolveAttack(request.AttackContext);
            sessionEvent = new SessionEvent(
                ActionEventTypes.Attack,
                $"Performed attack with {request.AttackContext.WeaponId}: {result.Message}",
                DateTime.UtcNow,
                AttackResult: result);
        }

        var newEventLog = new List<SessionEvent>(state.EventLog) { sessionEvent };
        var newState = state with { EventLog = newEventLog };
        sessionStore.Set(request.SessionId, newState);
        await persistence.SaveAsync(newState, cancellationToken);

        return Result<SessionEvent>.Ok(sessionEvent);
    }
}
