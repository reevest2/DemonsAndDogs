using MediatR;
using Models.GameSystems;

namespace Mediator.Mediator.Contracts.GameSystems;

public record ResolveAttackRequest(string SystemId, AttackContext Context)
    : IRequest<AttackResult>;
