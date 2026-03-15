using MediatR;
using Models.GameSystems;
using Models.Session;

namespace API.Services.Sessions.Contracts;

public record PerformActionRequest(
    string SessionId,
    ActionType ActionType,
    SkillCheckContext? SkillCheckContext = null,
    AttackContext? AttackContext = null)
    : IRequest<SessionEvent>;
