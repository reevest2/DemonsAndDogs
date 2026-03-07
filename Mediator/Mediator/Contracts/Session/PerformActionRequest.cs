using MediatR;
using Models.GameSystems;
using Models.Session;

namespace Mediator.Mediator.Contracts.Session;

public record PerformActionRequest(
    string SessionId,
    ActionType ActionType,
    SkillCheckContext? SkillCheckContext = null,
    AttackContext? AttackContext = null)
    : IRequest<SessionEvent>;
